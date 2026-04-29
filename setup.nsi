; NSIS installation script
; 班主任寄语与倒计时

Name "班主任寄语与倒计时"
OutFile "班主任寄语与倒计时安装包.exe"
InstallDir "$PROGRAMFILES\班主任寄语与倒计时"

; Define installation pages
Page directory
Page components
Page instfiles

; Define uninstallation pages
UninstPage uninstConfirm
UninstPage instfiles

; Installation section
Section "班主任寄语与倒计时"
  SetOutPath "$INSTDIR"
  ; Copy all files
  File /r "bin\Release\net6.0-windows\win-x64\publish\*"
  
  ; Create start menu shortcut
  CreateDirectory "$SMPROGRAMS\班主任寄语与倒计时"
  CreateShortcut "$SMPROGRAMS\班主任寄语与倒计时\班主任寄语与倒计时.lnk" "$INSTDIR\DesktopMessageApp.exe"
  
  ; Create desktop shortcut
  CreateShortcut "$DESKTOP\班主任寄语与倒计时.lnk" "$INSTDIR\DesktopMessageApp.exe"
SectionEnd

; Uninstallation section
Section "Uninstall"
  ; Delete start menu shortcut
  Delete "$SMPROGRAMS\班主任寄语与倒计时\班主任寄语与倒计时.lnk"
  RMDir "$SMPROGRAMS\班主任寄语与倒计时"
  
  ; Delete desktop shortcut
  Delete "$DESKTOP\班主任寄语与倒计时.lnk"
  
  ; Delete all files
  Delete /r "$INSTDIR\*"
  RMDir "$INSTDIR"
SectionEnd

; Run program after installation
Function .onInstSuccess
  Exec "$INSTDIR\DesktopMessageApp.exe"
FunctionEnd