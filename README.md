# Microsoft Edge 卸载工具 (WinForms)

基于 WinForms Framework 4.5 开发的 Microsoft Edge 完整卸载工具。

## 功能特性

- ✅ 完全卸载 Microsoft Edge 浏览器
- ✅ 卸载 WebView2 运行时（可选，带警告提示）
- ✅ 删除 Appx 应用包
- ✅ 清理注册表项
- ✅ 删除计划任务
- ✅ 清理快捷方式
- ✅ 删除系统残留文件
- ✅ 权限提升和所有权管理
- ✅ 进度显示和日志输出

## 项目结构

```
RemoveMSEdgeGUI/
├── Core/                    # 核心业务逻辑
│   ├── PermissionManager.cs # 权限管理
│   ├── EdgeRemover.cs       # 卸载核心逻辑
│   ├── FileCleaner.cs       # 文件清理
│   ├── RegistryCleaner.cs   # 注册表清理
│   ├── ServiceManager.cs    # 服务管理
│   ├── TaskSchedulerManager.cs # 计划任务管理
│   └── PowerShellHelper.cs  # PowerShell 辅助
├── Models/                  # 数据模型
│   ├── RemovalProgress.cs
│   └── RemovalResult.cs
├── Utils/                   # 工具类
│   └── NativeMethods.cs     # Windows API 声明
├── Properties/              # 项目属性
│   ├── AssemblyInfo.cs
│   ├── Resources.resx
│   ├── Resources.Designer.cs
│   ├── Settings.settings
│   └── Settings.Designer.cs
├── MainForm.cs             # 主窗体
├── MainForm.Designer.cs    # 主窗体设计器（内联）
├── MainForm.resx           # 资源文件
├── Program.cs              # 程序入口
├── App.config              # 应用配置
├── RemoveMSEdgeGUI.csproj  # 项目文件
└── RemoveMSEdgeGUI.sln      # 解决方案文件
```

## 技术栈

- **框架**: .NET Framework 4.5
- **UI**: Windows Forms
- **语言**: C# 5.0
- **IDE**: Visual Studio 2022

## 核心技术实现

### 1. 权限管理 (PermissionManager)

- UAC 权限提升
- 启用 `SeTakeOwnershipPrivilege` 权限
- 文件/目录所有权获取
- 访问权限完全控制授予

### 2. 文件清理 (FileCleaner)

- 递归删除受保护文件
- 清理 Edge 安装目录
- 清理用户数据目录
- 删除桌面和开始菜单快捷方式

### 3. 注册表清理 (RegistryCleaner)

- 清理 64 位和 32 位注册表键
- 递归删除注册表子键
- 处理 Appx 相关键值

### 4. 服务管理 (ServiceManager)

- 停止 Edge 相关 Windows 服务
- 终止 Edge 相关进程
- 进程和服务安全终止

### 5. 计划任务管理 (TaskSchedulerManager)

- 使用 Task Scheduler 2.0 COM 接口
- 遍历和删除 Edge 相关任务
- 处理动态 COM 对象

### 6. PowerShell 辅助 (PowerShellHelper)

- 调用 PowerShell 命令
- 获取已安装 Appx 包列表
- 移除 Appx 包

## 如何打开项目

1. 打开 Visual Studio 2022
2. 选择"打开项目或解决方案"
3. 导航到 `RemoveMSEdgeGUI.sln`
4. 点击"打开"

## 编译和运行

1. 在 Visual Studio 中按 `Ctrl+Shift+B` 编译项目
2. 按 `F5` 运行项目
3. 程序需要管理员权限运行

## 注意事项

⚠️ **警告**: 此工具将完全删除 Microsoft Edge 和相关组件，操作不可逆！

- 必须以管理员权限运行
- 卸载 WebView2 可能影响其他应用程序
- 建议在操作前备份重要数据
- 操作完成后建议重启计算机

## 系统要求

- Windows 10 或更高版本
- .NET Framework 4.5 或更高版本
- 管理员权限

## 许可证

本项目遵循原 Remove-MS-Edge 项目的许可证。
