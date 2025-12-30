using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace RemoveMSEdgeGUI.Core
{
    public class PermissionManager
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, out LUID lpLuid);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, bool DisableAllPrivileges, ref TOKEN_PRIVILEGES NewState, int BufferLength, IntPtr PreviousState, IntPtr ReturnLength);

        [StructLayout(LayoutKind.Sequential)]
        private struct LUID
        {
            public uint LowPart;
            public int HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TOKEN_PRIVILEGES
        {
            public LUID Luid;
            public uint Attributes;
            public int PrivilegeCount;
        }

        private const int TOKEN_QUERY = 0x0008;
        private const int TOKEN_ADJUST_PRIVILEGES = 0x0020;
        private const uint SE_PRIVILEGE_ENABLED = 0x00000002;
        private const string SE_TAKE_OWNERSHIP_NAME = "SeTakeOwnershipPrivilege";

        public static bool IsAdministrator()
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public static void ElevateUAC()
        {
            var startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            startInfo.Verb = "runas";

            try
            {
                Process.Start(startInfo);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                throw new Exception("无法提升权限: " + ex.Message);
            }
        }

        public static bool EnableTakeOwnershipPrivilege()
        {
            try
            {
                IntPtr tokenHandle;
                if (!OpenProcessToken(GetCurrentProcess(), TOKEN_QUERY | TOKEN_ADJUST_PRIVILEGES, out tokenHandle))
                {
                    return false;
                }

                LUID luid;
                if (!LookupPrivilegeValue(null, SE_TAKE_OWNERSHIP_NAME, out luid))
                {
                    CloseHandle(tokenHandle);
                    return false;
                }

                TOKEN_PRIVILEGES tokenPrivileges = new TOKEN_PRIVILEGES();
                tokenPrivileges.PrivilegeCount = 1;
                tokenPrivileges.Luid = luid;
                tokenPrivileges.Attributes = SE_PRIVILEGE_ENABLED;

                bool result = AdjustTokenPrivileges(tokenHandle, false, ref tokenPrivileges, 0, IntPtr.Zero, IntPtr.Zero);
                CloseHandle(tokenHandle);

                return result && Marshal.GetLastWin32Error() == 0;
            }
            catch
            {
                return false;
            }
        }

        public static bool TakeOwnership(string path)
        {
            try
            {
                System.Security.AccessControl.DirectorySecurity dirSecurity = null;
                System.Security.AccessControl.FileSecurity fileSecurity = null;
                bool isFile = File.Exists(path);

                if (isFile)
                {
                    var fileInfo = new System.IO.FileInfo(path);
                    fileSecurity = fileInfo.GetAccessControl();
                }
                else if (Directory.Exists(path))
                {
                    var directoryInfo = new System.IO.DirectoryInfo(path);
                    dirSecurity = directoryInfo.GetAccessControl();
                }
                else
                {
                    return false;
                }

                var identity = WindowsIdentity.GetCurrent().User;
                if (identity == null)
                    return false;

                if (isFile)
                {
                    fileSecurity.SetOwner(identity);
                    new FileInfo(path).SetAccessControl(fileSecurity);
                }
                else
                {
                    dirSecurity.SetOwner(identity);
                    new DirectoryInfo(path).SetAccessControl(dirSecurity);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool GrantFullControl(string path)
        {
            try
            {
                System.Security.AccessControl.DirectorySecurity dirSecurity = null;
                System.Security.AccessControl.FileSecurity fileSecurity = null;
                bool isFile = File.Exists(path);

                if (isFile)
                {
                    var fileInfo = new System.IO.FileInfo(path);
                    fileSecurity = fileInfo.GetAccessControl();
                }
                else if (Directory.Exists(path))
                {
                    var directoryInfo = new System.IO.DirectoryInfo(path);
                    dirSecurity = directoryInfo.GetAccessControl();
                }
                else
                {
                    return false;
                }

                var identity = WindowsIdentity.GetCurrent().User;
                if (identity == null)
                    return false;

                var rule = new System.Security.AccessControl.FileSystemAccessRule(
                    identity,
                    System.Security.AccessControl.FileSystemRights.FullControl,
                    System.Security.AccessControl.InheritanceFlags.ContainerInherit | System.Security.AccessControl.InheritanceFlags.ObjectInherit,
                    System.Security.AccessControl.PropagationFlags.None,
                    System.Security.AccessControl.AccessControlType.Allow
                );

                if (isFile)
                {
                    fileSecurity.AddAccessRule(rule);
                    new FileInfo(path).SetAccessControl(fileSecurity);
                }
                else
                {
                    dirSecurity.AddAccessRule(rule);
                    new DirectoryInfo(path).SetAccessControl(dirSecurity);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
