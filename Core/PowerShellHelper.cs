using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RemoveMSEdgeGUI.Core
{
    public class PowerShellHelper
    {
        public event Action<string, bool> OnLog;

        public void RemoveWebView2()
        {
            Log("使用 PowerShell 卸载 WebView2...", false);

            try
            {
                // Get all WebView2 packages
                var packages = GetInstalledPackages("*WebView2*");

                foreach (var package in packages)
                {
                    try
                    {
                        Log($"  正在移除包: {package}", false);
                        RemovePackage(package);
                    }
                    catch (Exception ex)
                    {
                        Log($"  移除包失败 {package}: {ex.Message}", true);
                    }
                }

                Log("WebView2 卸载完成", false);
            }
            catch (Exception ex)
            {
                Log($"  WebView2 卸载失败: {ex.Message}", true);
            }
        }

        public void RemoveEdgeAppxPackages()
        {
            Log("使用 PowerShell 移除 Edge Appx 包...", false);

            try
            {
                var packages = GetInstalledPackages("*Microsoft.Edge*");

                foreach (var package in packages)
                {
                    try
                    {
                        Log($"  正在移除包: {package}", false);
                        RemovePackage(package);
                    }
                    catch (Exception ex)
                    {
                        Log($"  移除包失败 {package}: {ex.Message}", true);
                    }
                }

                Log("Appx 包移除完成", false);
            }
            catch (Exception ex)
            {
                Log($"  Appx 包移除失败: {ex.Message}", true);
            }
        }

        private List<string> GetInstalledPackages(string pattern)
        {
            var packages = new List<string>();

            try
            {
                var script = $"Get-AppxPackage -Name '{pattern}' | Select-Object -ExpandProperty PackageFullName";
                var result = RunPowerShell(script);

                if (!string.IsNullOrEmpty(result))
                {
                    var lines = result.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        var trimmed = line.Trim();
                        if (!string.IsNullOrEmpty(trimmed))
                        {
                            packages.Add(trimmed);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"  获取包列表失败: {ex.Message}", true);
            }

            return packages;
        }

        private void RemovePackage(string packageFullName)
        {
            try
            {
                var script = $"Remove-AppxPackage -Package '{packageFullName}'";
                RunPowerShell(script);
            }
            catch (Exception ex)
            {
                throw new Exception($"无法移除包 {packageFullName}: {ex.Message}");
            }
        }

        private string RunPowerShell(string script)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{script}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                using (var process = Process.Start(psi))
                {
                    if (process == null)
                        return string.Empty;

                    var output = process.StandardOutput.ReadToEnd();
                    var error = process.StandardError.ReadToEnd();

                    process.WaitForExit(60000);

                    if (process.ExitCode != 0 && !string.IsNullOrEmpty(error))
                    {
                        Log($"    PowerShell 错误: {error}", true);
                    }

                    return output;
                }
            }
            catch (Exception ex)
            {
                Log($"  PowerShell 执行失败: {ex.Message}", true);
                return string.Empty;
            }
        }

        private void Log(string message, bool isError)
        {
            OnLog?.Invoke(message, isError);
        }
    }
}
