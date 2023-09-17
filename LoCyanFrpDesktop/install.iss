[Setup]
AppName=LoCyanFrp
AppVerName=LoCyanFrp - 1.2
DefaultDirName={pf}\LoCyanFrp
DefaultGroupName=LoCyanFrp
UninstallDisplayIcon={app}\LoCyanFrpDesktop.exe
Compression=lzma2
SolidCompression=yes
OutputDir=Output
AppPublisher=杭州樱芸网络科技有限公司
AppPublisherURL=https://www.locyanfrp.cn
PrivilegesRequired=admin

[Files]
Source: "D:\Frp\LoCyanFrpDesktop-Impl\LoCyanFrpDesktop\bin\Release\LoCyanFrpDesktop.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Frp\LoCyanFrpDesktop-Impl\LoCyanFrpDesktop\bin\Release\Microsoft.Exchange.WebServices.Auth.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Frp\LoCyanFrpDesktop-Impl\LoCyanFrpDesktop\bin\Release\Microsoft.Exchange.WebServices.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Frp\LoCyanFrpDesktop-Impl\LoCyanFrpDesktop\bin\Release\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Frp\LoCyanFrpDesktop-Impl\LoCyanFrpDesktop\bin\Release\frpc.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Frp\LoCyanFrpDesktop-Impl\LoCyanFrpDesktop\bin\Release\HandyControl.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Frp\LoCyanFrpDesktop-Impl\LoCyanFrpDesktop\resource\*"; DestDir: "{app}\resource"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\LoCyanFrp"; Filename: "{app}\LoCyanFrpDesktop.exe"; IconFilename: "{app}\resource\favicon.ico"
; Add other shortcuts you want to create here

[Icons]
Name: "{commondesktop}\LoCyanFrp"; Filename: "{app}\LoCyanFrpDesktop.exe"; IconFilename: "{app}\resource\favicon.ico";


[Run]
Filename: "{app}\LoCyanFrpDesktop.exe"; Description: "直接运行 LoCyanFrp"; Flags: postinstall nowait runascurrentuser

[Registry]
Root: HKCR; Subkey: "locyanfrp"; ValueType: string; ValueName: ""; ValueData: "LoCyanFrp Desktop Application Custom URL Scheme."
Root: HKCR; Subkey: "locyanfrp"; ValueType: string; ValueName: "URL Protocol"; ValueData: ""
Root: HKCR; Subkey: "locyanfrp\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\resource\favicon.ico"
Root: HKCR; Subkey: "locyanfrp\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\LoCyanFrpDesktop.exe"" ""%1"""