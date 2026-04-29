[Setup]
AppId={{B2B7E4F1-4A2E-4D9B-8F6B-9E8A7C3D2E1F}
AppName=班主任寄语和倒计时
AppVersion=1.0.0
AppPublisher=班主任寄语和倒计时
AppPublisherURL=https://example.com
AppSupportURL=https://example.com
AppUpdatesURL=https://example.com
DefaultDirName={pf}\班主任寄语和倒计时
DefaultGroupName=班主任寄语和倒计时
AllowNoIcons=no
LicenseFile=
OutputDir=C:\Users\zx\Desktop\待办事项\Output
OutputBaseFilename=DesktopMessageApp_Setup
Compression=lzma
SolidCompression=yes

[Languages]
Name: "chinese"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "C:\Users\zx\Desktop\待办事项\bin\Release\net6.0-windows\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\班主任寄语和倒计时"; Filename: "{app}\DesktopMessageApp.exe"
Name: "{commondesktop}\班主任寄语和倒计时"; Filename: "{app}\DesktopMessageApp.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\DesktopMessageApp.exe"; Description: "{cm:LaunchProgram,班主任寄语和倒计时}"; Flags: nowait postinstall skipifsilent