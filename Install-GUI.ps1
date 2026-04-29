# 班主任寄语与倒计时 - 图形界面安装脚本
Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

# 设置脚本编码
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

# 创建主窗口
$form = New-Object System.Windows.Forms.Form
$form.Text = "班主任寄语与倒计时 - 安装程序"
$form.Size = New-Object System.Drawing.Size(500, 300)
$form.StartPosition = "CenterScreen"
$form.FormBorderStyle = "FixedDialog"
$form.MaximizeBox = $false
$form.MinimizeBox = $false

# 创建标签
$label = New-Object System.Windows.Forms.Label
$label.Location = New-Object System.Drawing.Point(20, 30)
$label.Size = New-Object System.Drawing.Size(460, 30)
$label.Text = "请选择安装路径："
$label.Font = New-Object System.Drawing.Font("微软雅黑", 10, [System.Drawing.FontStyle]::Regular)
$form.Controls.Add($label)

# 创建文本框
$textBox = New-Object System.Windows.Forms.TextBox
$textBox.Location = New-Object System.Drawing.Point(20, 70)
$textBox.Size = New-Object System.Drawing.Size(400, 30)
$textBox.Font = New-Object System.Drawing.Font("微软雅黑", 10, [System.Drawing.FontStyle]::Regular)
$defaultPath = "$env:ProgramFiles\班主任寄语与倒计时"
$textBox.Text = $defaultPath
$form.Controls.Add($textBox)

# 创建浏览按钮
$browseButton = New-Object System.Windows.Forms.Button
$browseButton.Location = New-Object System.Drawing.Point(430, 70)
$browseButton.Size = New-Object System.Drawing.Size(50, 30)
$browseButton.Text = "浏览"
$browseButton.Font = New-Object System.Drawing.Font("微软雅黑", 9, [System.Drawing.FontStyle]::Regular)
$browseButton.Add_Click({
    $folderBrowser = New-Object System.Windows.Forms.FolderBrowserDialog
    $folderBrowser.Description = "选择安装目录"
    $folderBrowser.SelectedPath = $textBox.Text
    if ($folderBrowser.ShowDialog() -eq [System.Windows.Forms.DialogResult]::OK) {
        $textBox.Text = $folderBrowser.SelectedPath
    }
})
$form.Controls.Add($browseButton)

# 创建安装按钮
$installButton = New-Object System.Windows.Forms.Button
$installButton.Location = New-Object System.Drawing.Point(150, 150)
$installButton.Size = New-Object System.Drawing.Size(100, 40)
$installButton.Text = "安装"
$installButton.Font = New-Object System.Drawing.Font("微软雅黑", 10, [System.Drawing.FontStyle]::Bold)
$installButton.Add_Click({
    $installPath = $textBox.Text
    
    # 检查路径是否存在，不存在则创建
    if (-not (Test-Path $installPath)) {
        New-Item -ItemType Directory -Path $installPath -Force | Out-Null
    }
    
    # 复制文件
    $sourcePath = "bin\Release\net6.0-windows\win-x64\publish"
    Copy-Item -Path "$sourcePath\*" -Destination $installPath -Recurse -Force
    
    # 创建桌面快捷方式
    $desktopPath = [Environment]::GetFolderPath("Desktop")
    $shortcutPath = "$desktopPath\班主任寄语与倒计时.lnk"
    $shell = New-Object -ComObject WScript.Shell
    $shortcut = $shell.CreateShortcut($shortcutPath)
    $shortcut.TargetPath = "$installPath\DesktopMessageApp.exe"
    $shortcut.Save()
    
    # 创建开始菜单快捷方式
    $startMenuPath = "$env:APPDATA\Microsoft\Windows\Start Menu\Programs\班主任寄语与倒计时"
    if (-not (Test-Path $startMenuPath)) {
        New-Item -ItemType Directory -Path $startMenuPath -Force | Out-Null
    }
    $startMenuShortcut = "$startMenuPath\班主任寄语与倒计时.lnk"
    $shortcut2 = $shell.CreateShortcut($startMenuShortcut)
    $shortcut2.TargetPath = "$installPath\DesktopMessageApp.exe"
    $shortcut2.Save()
    
    # 显示安装完成消息
    [System.Windows.Forms.MessageBox]::Show("安装完成！

您可以通过以下方式启动程序：
1. 桌面快捷方式：班主任寄语与倒计时
2. 开始菜单：开始 > 所有程序 > 班主任寄语与倒计时
3. 直接运行：$installPath\DesktopMessageApp.exe", "安装完成", [System.Windows.Forms.MessageBoxButtons]::OK, [System.Windows.Forms.MessageBoxIcon]::Information)
    
    # 关闭窗口
    $form.Close()
})
$form.Controls.Add($installButton)

# 创建取消按钮
$cancelButton = New-Object System.Windows.Forms.Button
$cancelButton.Location = New-Object System.Drawing.Point(270, 150)
$cancelButton.Size = New-Object System.Drawing.Size(100, 40)
$cancelButton.Text = "取消"
$cancelButton.Font = New-Object System.Drawing.Font("微软雅黑", 10, [System.Drawing.FontStyle]::Regular)
$cancelButton.Add_Click({
    $form.Close()
})
$form.Controls.Add($cancelButton)

# 显示窗口
$form.ShowDialog() | Out-Null