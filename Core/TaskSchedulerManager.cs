using System;
using System.Runtime.InteropServices;

namespace RemoveMSEdgeGUI.Core
{
    public class TaskSchedulerManager
    {
        public event Action<string, bool> OnLog;

        // COM Interface definitions for Task Scheduler 2.0
        [ComImport]
        [Guid("BAE54997-48B1-4CBE-9965-D6BE263EBE4B")]
        private interface ITaskService
        {
            void Connect([MarshalAs(UnmanagedType.Bool)] bool forced = false, string server = null, string user = null, string domain = null, string password = null);
            ITaskFolder GetFolder(string path);
            ITaskDefinition NewTask(uint flags);
        }

        [ComImport]
        [Guid("F5BC8FC5-536D-4F77-B852-FBC1356FDEB6")]
        private interface ITaskDefinition
        {
        }

        [ComImport]
        [Guid("8CFAC062-A080-4C15-9A88-AA7C2AF80DF8")]
        private interface ITaskFolder
        {
            string Name { get; }
            string Path { get; }
            IRegisteredTaskCollection GetTasks(int flags);
            void DeleteTask(string name);
        }

        [ComImport]
        [Guid("9F833082-23FD-41D2-9453-4D04DD329688")]
        private interface IRegisteredTaskCollection
        {
            int Count { get; }
            IRegisteredTask get_Item(object index);
        }

        [ComImport]
        [Guid("6A67614B-F683-4D18-9A73-6B06E79D3B20")]
        private interface IRegisteredTask
        {
            string Name { get; }
            string Path { get; }
            void Delete();
        }

        public void RemoveEdgeTasks()
        {
            Log("删除 Edge 计划任务...", false);

            try
            {
                Type taskServiceType = Type.GetTypeFromProgID("Schedule.Service");
                dynamic taskService = Activator.CreateInstance(taskServiceType);
                taskService.Connect();

                dynamic rootFolder = taskService.GetFolder("\\");

                try
                {
                    dynamic tasks = rootFolder.GetTasks(1);

                    for (int i = 1; i <= tasks.Count; i++)
                    {
                        try
                        {
                            dynamic task = tasks.get_Item(i);
                            string taskName = task.Name;

                            if (IsEdgeTask(taskName))
                            {
                                Log($"  正在删除任务: {taskName}", false);
                                rootFolder.DeleteTask(taskName);
                                Log($"  已删除任务: {taskName}", false);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log($"  处理任务失败: {ex.Message}", true);
                        }
                    }
                }
                finally
                {
                    if (rootFolder != null)
                    {
                        Marshal.ReleaseComObject(rootFolder);
                    }
                    if (taskService != null)
                    {
                        Marshal.ReleaseComObject(taskService);
                    }
                }

                Log("计划任务删除完成", false);
            }
            catch (Exception ex)
            {
                Log($"  删除计划任务失败: {ex.Message}", true);
                Log("  请检查 Task Scheduler 服务是否正常运行", true);
            }
        }

        private bool IsEdgeTask(string taskName)
        {
            if (string.IsNullOrEmpty(taskName))
                return false;

            var edgeKeywords = new[] { "edge", "microsoftedge", "msedge", "edgeupdate" };

            foreach (var keyword in edgeKeywords)
            {
                if (taskName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
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
