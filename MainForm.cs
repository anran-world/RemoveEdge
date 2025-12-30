using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RemoveMSEdgeGUI.Core;

namespace RemoveMSEdgeGUI
{
    public partial class MainForm : Form
    {
        private EdgeRemover edgeRemover;
        private BackgroundWorker bgWorker;

        // UI Controls
        private GroupBox grpOptions;
        private CheckBox chkEdge;
        private CheckBox chkWebView;
        private CheckBox chkAppx;
        private CheckBox chkRegistry;
        private CheckBox chkTasks;
        private CheckBox chkShortcuts;
        private CheckBox chkSystemFiles;
        private Button btnRemove;
        private Button btnCancel;
        private ProgressBar progressBar;
        private RichTextBox txtLog;
        private Label lblTitle;
        private Label lblStatus;
        private Label lblWebViewInfo;
        private Label lblWarning;
        private LinkLabel linkDownloadEdge;
        private PictureBox pictureBox;

        public MainForm()
        {
            InitializeComponent();
            edgeRemover = new EdgeRemover();
            InitializeBackgroundWorker();
            InitializeEventHandlers();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.chkEdge = new System.Windows.Forms.CheckBox();
            this.chkWebView = new System.Windows.Forms.CheckBox();
            this.lblWebViewInfo = new System.Windows.Forms.Label();
            this.chkAppx = new System.Windows.Forms.CheckBox();
            this.chkRegistry = new System.Windows.Forms.CheckBox();
            this.chkTasks = new System.Windows.Forms.CheckBox();
            this.chkShortcuts = new System.Windows.Forms.CheckBox();
            this.chkSystemFiles = new System.Windows.Forms.CheckBox();
            this.lblWarning = new System.Windows.Forms.Label();
            this.linkDownloadEdge = new System.Windows.Forms.LinkLabel();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.grpOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft YaHei UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(400, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Microsoft Edge 完整卸载工具";
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(20, 60);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(300, 20);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "状态: 就绪";
            // 
            // grpOptions
            // 
            this.grpOptions.Controls.Add(this.chkEdge);
            this.grpOptions.Controls.Add(this.chkWebView);
            this.grpOptions.Controls.Add(this.lblWebViewInfo);
            this.grpOptions.Controls.Add(this.chkAppx);
            this.grpOptions.Controls.Add(this.chkRegistry);
            this.grpOptions.Controls.Add(this.chkTasks);
            this.grpOptions.Controls.Add(this.chkShortcuts);
            this.grpOptions.Controls.Add(this.chkSystemFiles);
            this.grpOptions.Controls.Add(this.lblWarning);
            this.grpOptions.Location = new System.Drawing.Point(20, 90);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(260, 400);
            this.grpOptions.TabIndex = 2;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "卸载选项";
            // 
            // chkEdge
            // 
            this.chkEdge.Checked = true;
            this.chkEdge.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEdge.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.chkEdge.Location = new System.Drawing.Point(15, 30);
            this.chkEdge.Name = "chkEdge";
            this.chkEdge.Size = new System.Drawing.Size(230, 24);
            this.chkEdge.TabIndex = 0;
            this.chkEdge.Text = "卸载 Edge 浏览器";
            this.chkEdge.CheckedChanged += new System.EventHandler(this.OnOptionChanged);
            // 
            // chkWebView
            // 
            this.chkWebView.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.chkWebView.Location = new System.Drawing.Point(15, 65);
            this.chkWebView.Name = "chkWebView";
            this.chkWebView.Size = new System.Drawing.Size(230, 24);
            this.chkWebView.TabIndex = 1;
            this.chkWebView.Text = "卸载 WebView2 运行时";
            this.chkWebView.CheckedChanged += new System.EventHandler(this.OnWebViewChanged);
            // 
            // lblWebViewInfo
            // 
            this.lblWebViewInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(100)))), ((int)(((byte)(0)))));
            this.lblWebViewInfo.Location = new System.Drawing.Point(35, 90);
            this.lblWebViewInfo.Name = "lblWebViewInfo";
            this.lblWebViewInfo.Size = new System.Drawing.Size(200, 20);
            this.lblWebViewInfo.TabIndex = 2;
            this.lblWebViewInfo.Text = "⚠ 可能影响其他应用";
            // 
            // chkAppx
            // 
            this.chkAppx.Checked = true;
            this.chkAppx.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAppx.Location = new System.Drawing.Point(15, 120);
            this.chkAppx.Name = "chkAppx";
            this.chkAppx.Size = new System.Drawing.Size(230, 24);
            this.chkAppx.TabIndex = 3;
            this.chkAppx.Text = "删除 Appx 应用包";
            this.chkAppx.CheckedChanged += new System.EventHandler(this.OnOptionChanged);
            // 
            // chkRegistry
            // 
            this.chkRegistry.Checked = true;
            this.chkRegistry.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRegistry.Location = new System.Drawing.Point(15, 155);
            this.chkRegistry.Name = "chkRegistry";
            this.chkRegistry.Size = new System.Drawing.Size(230, 24);
            this.chkRegistry.TabIndex = 4;
            this.chkRegistry.Text = "清理注册表项";
            this.chkRegistry.CheckedChanged += new System.EventHandler(this.OnOptionChanged);
            // 
            // chkTasks
            // 
            this.chkTasks.Checked = true;
            this.chkTasks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTasks.Location = new System.Drawing.Point(15, 190);
            this.chkTasks.Name = "chkTasks";
            this.chkTasks.Size = new System.Drawing.Size(230, 24);
            this.chkTasks.TabIndex = 5;
            this.chkTasks.Text = "删除计划任务";
            this.chkTasks.CheckedChanged += new System.EventHandler(this.OnOptionChanged);
            // 
            // chkShortcuts
            // 
            this.chkShortcuts.Checked = true;
            this.chkShortcuts.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShortcuts.Location = new System.Drawing.Point(15, 225);
            this.chkShortcuts.Name = "chkShortcuts";
            this.chkShortcuts.Size = new System.Drawing.Size(230, 24);
            this.chkShortcuts.TabIndex = 6;
            this.chkShortcuts.Text = "删除快捷方式";
            this.chkShortcuts.CheckedChanged += new System.EventHandler(this.OnOptionChanged);
            // 
            // chkSystemFiles
            // 
            this.chkSystemFiles.Checked = true;
            this.chkSystemFiles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSystemFiles.Location = new System.Drawing.Point(15, 260);
            this.chkSystemFiles.Name = "chkSystemFiles";
            this.chkSystemFiles.Size = new System.Drawing.Size(230, 24);
            this.chkSystemFiles.TabIndex = 7;
            this.chkSystemFiles.Text = "删除系统残留文件";
            this.chkSystemFiles.CheckedChanged += new System.EventHandler(this.OnOptionChanged);
            // 
            // lblWarning
            // 
            this.lblWarning.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblWarning.Location = new System.Drawing.Point(15, 300);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(230, 50);
            this.lblWarning.TabIndex = 8;
            this.lblWarning.Text = "⚠ 警告: 此操作不可逆\n请确保已备份重要数据";
            // 
            // linkDownloadEdge
            // 
            this.linkDownloadEdge.AutoSize = true;
            this.linkDownloadEdge.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.linkDownloadEdge.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.linkDownloadEdge.Location = new System.Drawing.Point(560, 25);
            this.linkDownloadEdge.Name = "linkDownloadEdge";
            this.linkDownloadEdge.Size = new System.Drawing.Size(220, 18);
            this.linkDownloadEdge.TabIndex = 9;
            this.linkDownloadEdge.TabStop = true;
            this.linkDownloadEdge.Text = "需要安装 Edge？点击下载";
            this.linkDownloadEdge.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnDownloadEdgeClicked);
            // 
            // btnRemove
            // 
            this.btnRemove.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnRemove.FlatAppearance.BorderSize = 0;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnRemove.ForeColor = System.Drawing.Color.White;
            this.btnRemove.Location = new System.Drawing.Point(300, 100);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(120, 40);
            this.btnRemove.TabIndex = 3;
            this.btnRemove.Text = "开始卸载";
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new System.EventHandler(this.OnRemoveClicked);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.btnCancel.Enabled = false;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(430, 100);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 40);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.OnCancelClicked);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(300, 155);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(470, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 5;
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.txtLog.Location = new System.Drawing.Point(300, 195);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(470, 295);
            this.txtLog.TabIndex = 6;
            this.txtLog.Text = "";
            // 
            // MainForm
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(801, 519);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.grpOptions);
            this.Controls.Add(this.linkDownloadEdge);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.txtLog);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Microsoft Edge 卸载工具";
            this.grpOptions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void InitializeBackgroundWorker()
        {
            bgWorker = new BackgroundWorker();
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.ProgressChanged += OnProgressChanged;
            bgWorker.RunWorkerCompleted += OnRunWorkerCompleted;
            bgWorker.DoWork += OnDoWork;
        }

        private void InitializeEventHandlers()
        {
            // 订阅 edgeRemover 事件，避免重复订阅
            edgeRemover.OnProgress += (progress, message) =>
            {
                this.Invoke(new Action(() =>
                {
                    progressBar.Value = progress;
                    lblStatus.Text = message;
                }));
            };

            edgeRemover.OnLog += (logMessage, isError) =>
            {
                var color = isError ? Color.LightCoral : Color.White;
                this.Invoke(new Action(() => AppendLog(logMessage, color)));
            };
        }

        private void OnOptionChanged(object sender, EventArgs e)
        {
            bool anyChecked = chkEdge.Checked || chkWebView.Checked || chkAppx.Checked ||
                            chkRegistry.Checked || chkTasks.Checked || chkShortcuts.Checked ||
                            chkSystemFiles.Checked;
            btnRemove.Enabled = anyChecked && !bgWorker.IsBusy;
        }

        private void OnWebViewChanged(object sender, EventArgs e)
        {
            if (chkWebView.Checked)
            {
                var result = MessageBox.Show(
                    "警告：卸载 WebView2 可能会导致以下应用无法正常运行：\n\n" +
                    "• Office 应用程序\n" +
                    "• 某些 Electron 应用（如 Discord, Teams）\n" +
                    "• 某些 UWP 应用\n" +
                    "• 依赖 WebView2 的其他软件\n\n" +
                    "确定要继续卸载吗？",
                    "WebView2 卸载警告",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.No)
                {
                    chkWebView.Checked = false;
                }
            }
            OnOptionChanged(sender, e);
        }

        private void OnRemoveClicked(object sender, EventArgs e)
        {
            var options = new RemovalOptions
            {
                RemoveEdge = chkEdge.Checked,
                RemoveWebView2 = chkWebView.Checked,
                RemoveAppxPackages = chkAppx.Checked,
                CleanRegistry = chkRegistry.Checked,
                RemoveTasks = chkTasks.Checked,
                RemoveShortcuts = chkShortcuts.Checked,
                RemoveSystemFiles = chkSystemFiles.Checked
            };

            // Confirm removal
            var message = "确定要卸载以下组件？\n\n";
            if (options.RemoveEdge) message += "✓ Edge 浏览器\n";
            if (options.RemoveWebView2) message += "✓ WebView2 运行时\n";
            if (options.RemoveAppxPackages) message += "✓ Appx 应用包\n";
            if (options.CleanRegistry) message += "✓ 注册表项\n";
            if (options.RemoveTasks) message += "✓ 计划任务\n";
            if (options.RemoveShortcuts) message += "✓ 快捷方式\n";
            if (options.RemoveSystemFiles) message += "✓ 系统残留文件\n";

            var result = MessageBox.Show(message, "确认卸载", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                return;
            }

            btnRemove.Enabled = false;
            btnCancel.Enabled = true;
            SetControlsEnabled(false);
            txtLog.Clear();
            progressBar.Value = 0;

            AppendLog("开始卸载流程...", Color.LightGreen);
            AppendLog($"当前时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", Color.Gray);

            bgWorker.RunWorkerAsync(options);
        }

        private void OnDoWork(object sender, DoWorkEventArgs e)
        {
            var options = (RemovalOptions)e.Argument;
            edgeRemover.RemoveAll(options, bgWorker);
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            if (bgWorker.IsBusy && bgWorker.WorkerSupportsCancellation)
            {
                bgWorker.CancelAsync();
                AppendLog("正在取消操作...", Color.Yellow);
            }
        }



        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = Math.Min(e.ProgressPercentage, 100);
            if (e.UserState != null)
            {
                lblStatus.Text = $"状态: {e.UserState}";
            }
        }

        private void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnRemove.Enabled = true;
            btnCancel.Enabled = false;
            SetControlsEnabled(true);

            if (e.Cancelled)
            {
                AppendLog("操作已取消", Color.Yellow);
                lblStatus.Text = "状态: 已取消";
            }
            else if (e.Error != null)
            {
                AppendLog($"错误: {e.Error.Message}", Color.LightCoral);
                AppendLog($"堆栈: {e.Error.StackTrace}", Color.LightCoral);
                lblStatus.Text = "状态: 发生错误";
                MessageBox.Show($"卸载过程中发生错误:\n{e.Error.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                AppendLog("卸载完成！", Color.LightGreen);
                AppendLog("建议重启计算机以确保所有更改生效。", Color.LightYellow);
                lblStatus.Text = "状态: 完成";
                progressBar.Value = 100;

                var result = MessageBox.Show(
                    "卸载完成！\n\n是否立即重启计算机？",
                    "完成",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );

                if (result == DialogResult.Yes)
                {
                    ProcessManager.RestartComputer();
                }
            }
        }

        private void SetControlsEnabled(bool enabled)
        {
            chkEdge.Enabled = enabled;
            chkWebView.Enabled = enabled;
            chkAppx.Enabled = enabled;
            chkRegistry.Enabled = enabled;
            chkTasks.Enabled = enabled;
            chkShortcuts.Enabled = enabled;
            chkSystemFiles.Enabled = enabled;
        }

        private void AppendLog(string message, Color color)
        {
            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.SelectionLength = 0;
            txtLog.SelectionColor = color;
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.ScrollToCaret();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AppendLog("Microsoft Edge 卸载工具已启动", Color.LightGreen);
            AppendLog($"操作系统: {Environment.OSVersion.VersionString}", Color.Gray);
            AppendLog($"框架版本: .NET Framework 4.5", Color.Gray);
        }

        private void OnDownloadEdgeClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://www.microsoft.com/zh-cn/edge/download");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法打开浏览器: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class RemovalOptions
    {
        public bool RemoveEdge { get; set; }
        public bool RemoveWebView2 { get; set; }
        public bool RemoveAppxPackages { get; set; }
        public bool CleanRegistry { get; set; }
        public bool RemoveTasks { get; set; }
        public bool RemoveShortcuts { get; set; }
        public bool RemoveSystemFiles { get; set; }
    }

    public static class ProcessManager
    {
        public static void RestartComputer()
        {
            try
            {
                System.Diagnostics.Process.Start("shutdown.exe", "/r /t 0");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法重启计算机: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
