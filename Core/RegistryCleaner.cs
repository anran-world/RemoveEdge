using System;
using Microsoft.Win32;

namespace RemoveMSEdgeGUI.Core
{
    public class RegistryCleaner
    {
        public event Action<string, bool> OnLog;

        private string[] registryKeys = new[]
        {
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\msedge.exe",
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\msedgewebview2.exe",
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Edge",
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\EdgeUpdate",
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\EdgeWebView",
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Microsoft Edge",
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Microsoft Edge Update",
            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Microsoft EdgeWebView",
            @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Edge",
            @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate",
            @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\EdgeWebView",
            @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Microsoft Edge",
            @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Microsoft Edge Update",
            @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Microsoft EdgeWebView",
            @"HKEY_CLASSES_ROOT\MSEdgeHTM",
            @"HKEY_CLASSES_ROOT\MSEdgeMHT",
            @"HKEY_CLASSES_ROOT\msedge",
            @"HKEY_CLASSES_ROOT\Microsoft.Edge",
            @"HKEY_CURRENT_USER\Software\Microsoft\Edge",
            @"HKEY_CURRENT_USER\Software\Microsoft\EdgeUpdate",
            @"HKEY_CURRENT_USER\Software\Microsoft\EdgeWebView"
        };

        public void CleanEdgeRegistry()
        {
            Log("开始清理 Edge 注册表项...", false);

            foreach (var keyPath in registryKeys)
            {
                try
                {
                    var hive = GetRegistryHive(keyPath);
                    var keyName = GetRegistryKeyName(keyPath);

                    if (!string.IsNullOrEmpty(keyName))
                    {
                        TryDeleteKey(hive, keyName);
                    }
                }
                catch (Exception ex)
                {
                    Log($"  处理注册表键失败 {keyPath}: {ex.Message}", true);
                }
            }

            Log("注册表清理完成", false);
        }

        private RegistryHive GetRegistryHive(string keyPath)
        {
            if (keyPath.StartsWith(@"HKEY_LOCAL_MACHINE\", StringComparison.OrdinalIgnoreCase))
                return RegistryHive.LocalMachine;
            else if (keyPath.StartsWith(@"HKEY_CURRENT_USER\", StringComparison.OrdinalIgnoreCase))
                return RegistryHive.CurrentUser;
            else if (keyPath.StartsWith(@"HKEY_CLASSES_ROOT\", StringComparison.OrdinalIgnoreCase))
                return RegistryHive.ClassesRoot;
            else
                return RegistryHive.LocalMachine;
        }

        private string GetRegistryKeyName(string keyPath)
        {
            var separatorIndex = keyPath.IndexOf('\\');
            if (separatorIndex >= 0 && keyPath.Length > separatorIndex + 1)
            {
                return keyPath.Substring(separatorIndex + 1);
            }
            return null;
        }

        private void TryDeleteKey(RegistryHive hive, string keyName)
        {
            try
            {
                using (var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64))
                {
                    DeleteKeyRecursive(baseKey, keyName);
                }

                using (var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry32))
                {
                    DeleteKeyRecursive(baseKey, keyName);
                }
            }
            catch (Exception ex)
            {
                Log($"    删除注册表键失败 {keyName}: {ex.Message}", true);
            }
        }

        private void DeleteKeyRecursive(RegistryKey baseKey, string keyName)
        {
            try
            {
                bool keyExists = false;
                var subKeyNames = new string[0];

                // First check if key exists and get subkeys
                using (var key = baseKey.OpenSubKey(keyName, true))
                {
                    if (key != null)
                    {
                        keyExists = true;
                        subKeyNames = key.GetSubKeyNames();
                    }
                }

                if (!keyExists)
                    return;

                // Recursively delete subkeys
                using (var key = baseKey.OpenSubKey(keyName, true))
                {
                    foreach (var subKeyName in subKeyNames)
                    {
                        try
                        {
                            DeleteKeyRecursive(key, subKeyName);
                        }
                        catch
                        {
                            // Continue with other subkeys
                        }
                    }
                }

                // Finally delete the key itself
                baseKey.DeleteSubKeyTree(keyName);
                Log($"  已删除注册表键: {keyName}", false);
            }
            catch (Exception ex)
            {
                Log($"    删除注册表键失败 {keyName}: {ex.Message}", true);
            }
        }

        private void Log(string message, bool isError)
        {
            OnLog?.Invoke(message, isError);
        }
    }
}
