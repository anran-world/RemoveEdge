using System;
using System.IO;

namespace RemoveMSEdgeGUI.Core
{
    public class FileCleaner
    {
        public event Action<string, bool> OnLog;

        private string[] edgeDirectories = new[]
        {
            @"C:\Program Files (x86)\Microsoft\Edge",
            @"C:\Program Files\Microsoft\Edge",
            @"C:\Program Files (x86)\Microsoft\EdgeUpdate",
            @"C:\Program Files\Microsoft\EdgeUpdate",
            @"C:\Program Files (x86)\Microsoft\EdgeCore",
            @"C:\Program Files\Microsoft\EdgeCore",
            @"C:\Program Files (x86)\Microsoft\EdgeWebView",
            @"C:\Program Files\Microsoft\EdgeWebView",
            @"C:\ProgramData\Microsoft\EdgeUpdate",
            @"C:\ProgramData\Microsoft\EdgeCore"
        };

        private string[] userDataDirectories = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\Edge"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\EdgeUpdate"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\EdgeWebView"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Microsoft\Edge")
        };

        private string[] shortcutPaths = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Microsoft Edge.lnk"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory), "Microsoft Edge.lnk"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "Microsoft Edge.lnk"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms), "Microsoft Edge.lnk")
        };

        public void CleanSystemFiles()
        {
            Log("清理 Edge 系统文件...", false);

            foreach (var dir in edgeDirectories)
            {
                TryDeleteDirectory(dir);
            }

            foreach (var dir in userDataDirectories)
            {
                TryDeleteDirectory(dir);
            }

            Log("系统文件清理完成", false);
        }

        public void RemoveShortcuts()
        {
            Log("删除 Edge 快捷方式...", false);

            foreach (var shortcutPath in shortcutPaths)
            {
                try
                {
                    if (File.Exists(shortcutPath))
                    {
                        File.Delete(shortcutPath);
                        Log($"  已删除: {shortcutPath}", false);
                    }
                }
                catch (Exception ex)
                {
                    Log($"  删除失败 {shortcutPath}: {ex.Message}", true);
                }
            }

            // Remove from Start Menu
            RemoveFromStartMenu();

            Log("快捷方式删除完成", false);
        }

        private void TryDeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Log($"  正在删除: {path}", false);
                    DeleteDirectoryRecursive(path);
                    Log($"  已删除: {path}", false);
                }
            }
            catch (Exception ex)
            {
                Log($"  删除失败 {path}: {ex.Message}", true);
            }
        }

        private void DeleteDirectoryRecursive(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                    return;

                PermissionManager.TakeOwnership(path);
                PermissionManager.GrantFullControl(path);

                var dirs = Directory.GetDirectories(path);
                foreach (var dir in dirs)
                {
                    DeleteDirectoryRecursive(dir);
                }

                var files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    try
                    {
                        PermissionManager.TakeOwnership(file);
                        PermissionManager.GrantFullControl(file);
                        File.SetAttributes(file, FileAttributes.Normal);
                        File.Delete(file);
                    }
                    catch
                    {
                        // Ignore
                    }
                }

                Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
                Log($"    删除子目录失败: {ex.Message}", true);
            }
        }

        private void RemoveFromStartMenu()
        {
            try
            {
                var startMenuPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu),
                    "Programs"
                );

                if (Directory.Exists(startMenuPath))
                {
                    // Use manual recursion instead of SearchOption.AllDirectories for better error handling
                    RemoveShortcutsRecursive(startMenuPath, "*Edge*.lnk");
                }
            }
            catch (Exception ex)
            {
                Log($"  清理开始菜单失败: {ex.Message}", true);
            }
        }

        private void RemoveShortcutsRecursive(string directory, string pattern)
        {
            try
            {
                if (!Directory.Exists(directory))
                    return;

                // Remove shortcuts in current directory
                var shortcuts = Directory.GetFiles(directory, pattern);
                foreach (var shortcut in shortcuts)
                {
                    try
                    {
                        File.Delete(shortcut);
                        Log($"  已删除开始菜单快捷方式: {shortcut}", false);
                    }
                    catch (Exception ex)
                    {
                        Log($"  删除失败 {shortcut}: {ex.Message}", true);
                    }
                }

                // Recursively process subdirectories
                var subdirectories = Directory.GetDirectories(directory);
                foreach (var subdirectory in subdirectories)
                {
                    try
                    {
                        RemoveShortcutsRecursive(subdirectory, pattern);
                    }
                    catch
                    {
                        // Continue with other directories
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"  处理目录失败 {directory}: {ex.Message}", true);
            }
        }

        private void Log(string message, bool isError)
        {
            OnLog?.Invoke(message, isError);
        }
    }
}
