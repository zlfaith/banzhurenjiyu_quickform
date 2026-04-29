# 班主任寄语与倒计时软件

一款基于 WPF 的 Windows 桌面软件，用于展示班主任每日寄语和中考/高考倒计时。

## 功能特点

- 📝 **每日寄语**：根据星期几自动切换不同的班主任寄语
- ⏱️ **倒计时显示**：设置目标日期，实时显示剩余天数
- 🔒 **半透明窗口**：返回桌面即可看到，不影响其他操作
- 🖱️ **可拖动窗口**：自由调整显示位置
- 💾 **自动保存配置**：配置信息自动保存到本地
- 🔄 **远程更新**：支持从云端获取最新配置

## 技术栈

- **框架**: .NET 6.0 (WPF)
- **语言**: C#
- **JSON解析**: Newtonsoft.Json
- **目标平台**: Windows (x64)

## 快速开始

### 运行已编译版本

直接运行 `bin/Release/net6.0-windows/win-x64/DesktopMessageApp.exe`

### 开发构建

```bash
# 确保安装了 .NET 6.0 SDK
dotnet --version

# 构建项目
dotnet build

# 运行项目
dotnet run
```

### 发布项目

```bash
# 发布为自包含应用
dotnet publish -c Release -r win-x64 --self-contained true
```

## 使用说明

1. **启动软件**：运行 `DesktopMessageApp.exe`
2. **查看寄语**：主窗口显示当日班主任寄语
3. **查看倒计时**：显示距离目标日期的剩余天数
4. **设置**：点击"设置"按钮修改寄语和目标日期
5. **关闭**：点击"关闭"按钮退出软件
6. **拖动窗口**：点击窗口任意位置拖动调整显示位置

## 配置说明

配置文件存储在 `%LOCALAPPDATA%\班主任寄语与倒计时\config.txt`

配置格式：
```
倒计时名称
正文字体大小
倒计时字体大小
对齐方式(Left/Center/Right)
寄语内容1
寄语内容2
...
目标日期(yyyy/MM/dd)
```

## 项目结构

```
├── App.xaml              # 应用程序入口配置
├── App.xaml.cs           # 应用程序逻辑
├── MainWindow.xaml       # 主窗口界面
├── MainWindow.xaml.cs    # 主窗口逻辑
├── SettingsWindow.xaml   # 设置窗口界面
├── SettingsWindow.xaml.cs# 设置窗口逻辑
├── QuickFormClient.cs    # 远程配置客户端
├── DesktopMessageApp.csproj # 项目配置
└── README.md             # 项目说明
```

## 功能说明

### 每日寄语切换

软件根据当前星期几自动选择对应的寄语：
- 周一 → 第1条寄语
- 周二 → 第2条寄语
- ...
- 周日 → 第7条寄语

### 倒计时功能

- 显示距离目标日期的剩余天数
- 支持自定义倒计时名称（如"中考"、"高考"等）
- 目标日期到达后显示"已到达"提示

### 界面自定义

- 支持调整正文字体大小
- 支持调整倒计时字体大小
- 支持文字对齐方式（左对齐/居中/右对齐）

## 许可证

MIT License