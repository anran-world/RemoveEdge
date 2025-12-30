using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace RemoveMSEdgeGUI.Core
{
    public class ServiceManager
    {
        public event Action<string, bool> OnLog;

        private string[] edgeProcessNames = new[]
        {
            "msedge",
            "msedgewebview2",
            "MicrosoftEdgeUpdate",
            "edgeupdater"
        };

        private string[] edgeServiceNames = new[]
        {
            "MicrosoftEdgeElevationService",
            "MicrosoftEdgeUpdateService",
            "edgeupdate",
            "MicrosoftEdgeUpdate",
            "MicrosoftEdgeUpdate3",
            "MicrosoftEdgeUpdateVM",
            "MicrosoftEdgeSHIMService",
            "MicrosoftEdgeUpdateServiceUA"
        };

        public void StopEdgeServices()
        {
            Log("停止 Edge 相关服务...", false);

            try
            {
                var services = ServiceController.GetServices();
                foreach (var service in services)
                {
                    try
                    {
                        if (IsEdgeService(service.ServiceName))
                        {
                            if (service.Status == ServiceControllerStatus.Running)
                            {
                                Log($"  正在停止服务: {service.ServiceName}", false);
                                service.Stop();
                                service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                                Log($"  已停止服务: {service.ServiceName}", false);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log($"  停止服务失败 {service.ServiceName}: {ex.Message}", true);
                    }
                    finally
                    {
                        service.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"  停止服务过程失败: {ex.Message}", true);
            }

            Log("服务停止完成", false);
        }

        public void KillEdgeProcesses()
        {
            Log("终止 Edge 相关进程...", false);

            foreach (var processName in edgeProcessNames)
            {
                try
                {
                    var processes = Process.GetProcessesByName(processName);
                    foreach (var process in processes)
                    {
                        try
                        {
                            Log($"  正在终止进程: {processName} (PID: {process.Id})", false);
                            process.Kill();
                            process.WaitForExit(5000);
                            Log($"  已终止进程: {processName}", false);
                        }
                        catch (Exception ex)
                        {
                            Log($"  终止进程失败 {processName}: {ex.Message}", true);
                        }
                        finally
                        {
                            process.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log($"  查找进程失败 {processName}: {ex.Message}", true);
                }
            }

            Log("进程终止完成", false);
        }

        private bool IsEdgeService(string serviceName)
        {
            foreach (var edgeService in edgeServiceNames)
            {
                if (serviceName.IndexOf(edgeService, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void Log(string message, bool isError)
        {
            OnLog?.Invoke(message, isError);
        }
    }
}
