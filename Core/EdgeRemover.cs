using System;
using System.ComponentModel;
using System.Threading;

namespace RemoveMSEdgeGUI.Core
{
    public class EdgeRemover
    {
        public event Action<int, string> OnProgress;
        public event Action<string, bool> OnLog;

        private FileCleaner fileCleaner;
        private RegistryCleaner registryCleaner;
        private ServiceManager serviceManager;
        private TaskSchedulerManager taskScheduler;
        private PowerShellHelper powerShellHelper;

        public EdgeRemover()
        {
            fileCleaner = new FileCleaner();
            registryCleaner = new RegistryCleaner();
            serviceManager = new ServiceManager();
            taskScheduler = new TaskSchedulerManager();
            powerShellHelper = new PowerShellHelper();

            fileCleaner.OnLog += OnLog;
            registryCleaner.OnLog += OnLog;
            serviceManager.OnLog += OnLog;
            taskScheduler.OnLog += OnLog;
            powerShellHelper.OnLog += OnLog;
        }

        public void RemoveAll(RemovalOptions options, BackgroundWorker worker)
        {
            int totalSteps = 0;
            int currentStep = 0;

            if (options.RemoveEdge) totalSteps++;
            if (options.RemoveWebView2) totalSteps++;
            if (options.RemoveAppxPackages) totalSteps++;
            if (options.CleanRegistry) totalSteps++;
            if (options.RemoveTasks) totalSteps++;
            if (options.RemoveShortcuts) totalSteps++;
            if (options.RemoveSystemFiles) totalSteps++;

            if (totalSteps == 0)
                return;

            // Use float for more accurate progress calculation
            double progressPerStep = 100.0 / totalSteps;

            // Enable privileges
            Log("正在启用管理员权限...", false);
            if (!PermissionManager.EnableTakeOwnershipPrivilege())
            {
                Log("  警告: 无法启用所有权特权，某些文件可能无法删除", true);
            }

            // Stop services first
            Log("正在停止 Edge 相关服务...", false);
            serviceManager.StopEdgeServices();

            // Kill processes
            Log("正在终止 Edge 进程...", false);
            serviceManager.KillEdgeProcesses();

            // Remove Edge Browser
            if (options.RemoveEdge && !worker.CancellationPending)
            {
                Log("正在卸载 Edge 浏览器...", false);
                RemoveEdgeBrowser();
                UpdateProgress(ref currentStep, progressPerStep, "卸载 Edge 浏览器");
            }

            // Remove WebView2
            if (options.RemoveWebView2 && !worker.CancellationPending)
            {
                Log("正在卸载 WebView2 运行时...", false);
                RemoveWebView2();
                UpdateProgress(ref currentStep, progressPerStep, "卸载 WebView2 运行时");
            }

            // Remove Appx packages
            if (options.RemoveAppxPackages && !worker.CancellationPending)
            {
                Log("正在删除 Appx 应用包...", false);
                RemoveAppxPackages();
                UpdateProgress(ref currentStep, progressPerStep, "删除 Appx 应用包");
            }

            // Clean registry
            if (options.CleanRegistry && !worker.CancellationPending)
            {
                Log("正在清理注册表...", false);
                registryCleaner.CleanEdgeRegistry();
                UpdateProgress(ref currentStep, progressPerStep, "清理注册表");
            }

            // Remove tasks
            if (options.RemoveTasks && !worker.CancellationPending)
            {
                Log("正在删除计划任务...", false);
                taskScheduler.RemoveEdgeTasks();
                UpdateProgress(ref currentStep, progressPerStep, "删除计划任务");
            }

            // Remove shortcuts
            if (options.RemoveShortcuts && !worker.CancellationPending)
            {
                Log("正在删除快捷方式...", false);
                fileCleaner.RemoveShortcuts();
                UpdateProgress(ref currentStep, progressPerStep, "删除快捷方式");
            }

            // Remove system files
            if (options.RemoveSystemFiles && !worker.CancellationPending)
            {
                Log("正在删除系统残留文件...", false);
                fileCleaner.CleanSystemFiles();
                UpdateProgress(ref currentStep, progressPerStep, "删除系统残留文件");
            }
        }

        private void RemoveEdgeBrowser()
        {
            try
            {
                Log("  查找 Edge 安装目录...", false);
                var edgePaths = new[]
                {
                    @"C:\Program Files (x86)\Microsoft\Edge\Application",
                    @"C:\Program Files\Microsoft\Edge\Application",
                    @"C:\Program Files (x86)\Microsoft\EdgeUpdate",
                    @"C:\Program Files\Microsoft\EdgeUpdate"
                };

                foreach (var path in edgePaths)
                {
                    if (System.IO.Directory.Exists(path))
                    {
                        Log($"  删除: {path}", false);
                        DeleteDirectoryRecursive(path);
                    }
                }

                Log("  运行 Edge 卸载程序...", false);
                var uninstaller = FindEdgeUninstaller();
                if (uninstaller != null)
                {
                    RunUninstaller(uninstaller, "--force-uninstall");
                }

                Log("  Edge 浏览器卸载完成", false);
            }
            catch (Exception ex)
            {
                Log($"  错误: {ex.Message}", true);
            }
        }

        private void RemoveWebView2()
        {
            try
            {
                Log("  查找 WebView2 安装目录...", false);
                var webviewPaths = new[]
                {
                    @"C:\Program Files (x86)\Microsoft\EdgeWebView\Application",
                    @"C:\Program Files\Microsoft\EdgeWebView\Application"
                };

                foreach (var path in webviewPaths)
                {
                    if (System.IO.Directory.Exists(path))
                    {
                        Log($"  删除: {path}", false);
                        DeleteDirectoryRecursive(path);
                    }
                }

                Log("  使用 PowerShell 卸载 WebView2...", false);
                powerShellHelper.RemoveWebView2();

                Log("  WebView2 卸载完成", false);
            }
            catch (Exception ex)
            {
                Log($"  错误: {ex.Message}", true);
            }
        }

        private void RemoveAppxPackages()
        {
            try
            {
                Log("  使用 PowerShell 移除 Edge Appx 包...", false);
                powerShellHelper.RemoveEdgeAppxPackages();
                Log("  Appx 包移除完成", false);
            }
            catch (Exception ex)
            {
                Log($"  错误: {ex.Message}", true);
            }
        }

        private string FindEdgeUninstaller()
        {
            try
            {
                var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                var edgePath = System.IO.Path.Combine(programFiles, @"Microsoft\Edge\Application");
                
                if (!System.IO.Directory.Exists(edgePath))
                    edgePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Microsoft\Edge\Application");

                if (System.IO.Directory.Exists(edgePath))
                {
                    var versionDirs = System.IO.Directory.GetDirectories(edgePath);
                    foreach (var versionDir in versionDirs)
                    {
                        var uninstaller = System.IO.Path.Combine(versionDir, "setup.exe");
                        if (System.IO.File.Exists(uninstaller))
                        {
                            return uninstaller;
                        }
                    }
                }
            }
            catch
            {
                // Ignore
            }

            return null;
        }

        private void RunUninstaller(string uninstaller, string args)
        {
            try
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = uninstaller,
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                };

                using (var process = System.Diagnostics.Process.Start(startInfo))
                {
                    if (process != null)
                    {
                        process.WaitForExit(60000);
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"  卸载程序执行失败: {ex.Message}", true);
            }
        }

        private void DeleteDirectoryRecursive(string path)
        {
            try
            {
                if (!System.IO.Directory.Exists(path))
                    return;

                PermissionManager.TakeOwnership(path);
                PermissionManager.GrantFullControl(path);

                var dirs = System.IO.Directory.GetDirectories(path);
                foreach (var dir in dirs)
                {
                    DeleteDirectoryRecursive(dir);
                }

                var files = System.IO.Directory.GetFiles(path);
                var filesDeleted = 0;
                foreach (var file in files)
                {
                    try
                    {
                        PermissionManager.TakeOwnership(file);
                        PermissionManager.GrantFullControl(file);
                        System.IO.File.SetAttributes(file, System.IO.FileAttributes.Normal);
                        System.IO.File.Delete(file);
                        filesDeleted++;
                    }
                    catch
                    {
                        // Ignore, continue with other files
                    }
                }

                // Only try to delete directory if we deleted some files or it's empty
                if (filesDeleted == 0 && files.Length > 0)
                {
                    Log($"  警告: 无法删除目录中的文件，跳过目录 {path}", true);
                    return;
                }

                System.IO.Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
                Log($"  删除目录失败 {path}: {ex.Message}", true);
            }
        }

        private void UpdateProgress(ref int currentStep, double progressPerStep, string stepName)
        {
            currentStep++;
            int progress = Math.Min(100, (int)(currentStep * progressPerStep));
            OnProgress?.Invoke(progress, stepName);
        }

        private void Log(string message, bool isError)
        {
            OnLog?.Invoke(message, isError);
        }
    }
}
