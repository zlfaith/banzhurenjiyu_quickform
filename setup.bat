@echo off

REM 检查是否已安装 .NET SDK
echo 检查 .NET SDK 安装情况...
where dotnet >nul 2>nul
if %errorlevel% equ 0 (
    echo .NET SDK 已安装，跳过安装步骤
) else (
    echo 正在打开 .NET SDK 下载页面...
    start "" "https://dotnet.microsoft.com/zh-cn/download/dotnet/6.0"
    
    echo 请下载并安装 .NET 6.0 SDK，然后再次运行此脚本
    pause
    exit
)

REM 构建项目
echo 正在构建项目...
dotnet build

REM 运行软件
echo 正在启动软件...
dotnet run

pause