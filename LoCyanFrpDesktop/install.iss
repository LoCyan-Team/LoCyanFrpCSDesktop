[Setup]
AppName=LoCyanFrp
AppVersion=1.0
DefaultDirName={pf}\LoCyanFrp
DefaultGroupName=LoCyanFrp
UninstallDisplayIcon={app}\LoCyanFrpDesktop.exe
Compression=lzma2
SolidCompression=yes
OutputDir=Output
AppPublisher=����ӣܿ����Ƽ����޹�˾
AppPublisherURL=https://www.locyan.cn
PrivilegesRequired=admin

[Files]
Source: "C:\Frp\LoCyanFrpDesktop-Impl\LoCyanFrpDesktop\bin\Release\LoCyanFrpDesktop.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Frp\LoCyanFrpDesktop-Impl\LoCyanFrpDesktop\bin\Release\LoCyanFrpDesktop.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Frp\LoCyanFrpDesktop-Impl\LoCyanFrpDesktop\bin\Release\Microsoft.Exchange.WebServices.Auth.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Frp\LoCyanFrpDesktop-Impl\LoCyanFrpDesktop\bin\Release\Microsoft.Exchange.WebServices.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Frp\LoCyanFrpDesktop-Impl\LoCyanFrpDesktop\bin\Release\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Frp\LoCyanFrpDesktop-Impl\LoCyanFrpDesktop\bin\Release\frpc.exe"; DestDir: "{app}"; Flags: ignoreversion


[Icons]
Name: "{group}\LoCyanFrp"; Filename: "{app}\LoCyanFrpDesktop.exe"; IconFilename: "C:\Frp\LoCyanFrpDesktop-Impl\LoCyanFrpDesktop\resource\favicon.ico"
; Add other shortcuts you want to create here

[Icons]
Name: "{commondesktop}\LoCyanFrp"; Filename: "{app}\LoCyanFrpDesktop.exe"; IconFilename: "C:\Frp\LoCyanFrpDesktop-Impl\LoCyanFrpDesktop\resource\favicon.ico";


[Run]
Filename: "{app}\LoCyanFrpDesktop.exe"; Description: "ֱ������ LoCyanFrp"; Flags: postinstall nowait runascurrentuser

