@echo off
chcp 65001
echo 班主任寄语与倒计时 - 安装程序
echo ===============================
echo.

:: 默认安装路径
set default_install_dir=%ProgramFiles%\班主任寄语与倒计时

:: 提示用户输入安装路径
echo 请输入安装路径（默认：%default_install_dir%）:
set /p install_dir=

:: 如果用户未输入，使用默认路径
if "%install_dir%"=="" set install_dir=%default_install_dir%

echo.
echo 安装路径: %install_dir%
echo 正在安装...

:: 创建安装目录
if not exist "%install_dir%" mkdir "%install_dir%"

:: 复制所有文件
echo 正在复制文件...
xcopy "bin\Release\net6.0-windows\win-x64\publish\*" "%install_dir%" /E /Y

:: 创建桌面快捷方式
echo 正在创建桌面快捷方式...
set shortcut_name=班主任寄语与倒计时.lnk
set shortcut_path=%USERPROFILE%\Desktop\%shortcut_name%

:: 使用PowerShell创建快捷方式
powershell -Command "$WshShell = New-Object -ComObject WScript.Shell; $Shortcut = $WshShell.CreateShortcut('%shortcut_path%'); $Shortcut.TargetPath = '%install_dir%\DesktopMessageApp.exe'; $Shortcut.Save()"

:: 创建开始菜单快捷方式
echo 正在创建开始菜单快捷方式...
set start_menu_path=%APPDATA%\Microsoft\Windows\Start Menu\Programs\班主任寄语与倒计时
if not exist "%start_menu_path%" mkdir "%start_menu_path%"
set start_menu_shortcut=%start_menu_path%\班主任寄语与倒计时.lnk

:: 使用PowerShell创建开始菜单快捷方式
powershell -Command "$WshShell = New-Object -ComObject WScript.Shell; $Shortcut = $WshShell.CreateShortcut('%start_menu_shortcut%'); $Shortcut.TargetPath = '%install_dir%\DesktopMessageApp.exe'; $Shortcut.Save()"

echo.
echo 安装完成！
echo 您可以通过以下方式启动程序：
echo 1. 桌面快捷方式：%shortcut_name%
echo 2. 开始菜单：开始 ^> 所有程序 ^> 班主任寄语与倒计时
echo 3. 直接运行：%install_dir%\DesktopMessageApp.exe
echo.
echo 按任意键退出...
pause > nul