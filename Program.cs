using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using RemoveMSEdgeGUI.Core;

namespace RemoveMSEdgeGUI
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // 检查管理员权限
            if (!PermissionManager.IsAdministrator())
            {
                var result = MessageBox.Show(
                    "此程序需要管理员权限才能正常运行。\n\n是否以管理员身份重新启动？",
                    "需要管理员权限",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    PermissionManager.ElevateUAC();
                }
                return;
            }

            //// 加载内嵌的配置文件
            //LoadEmbeddedConfig();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void LoadEmbeddedConfig()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var configName = assembly.GetName().Name + ".exe.config";
                var resourceName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(r => r.EndsWith(configName));

                if (resourceName != null)
                {
                    using (var stream = assembly.GetManifestResourceStream(resourceName))
                    using (var reader = new StreamReader(stream))
                    {
                        var configContent = reader.ReadToEnd();
                        var tempConfigPath = Path.Combine(Path.GetTempPath(), configName);
                        File.WriteAllText(tempConfigPath, configContent);

                        // 设置配置文件路径
                        AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", tempConfigPath);
                    }
                }
            }
            catch
            {
                // 如果加载失败，忽略错误，使用默认配置
            }
        }
    }
}
