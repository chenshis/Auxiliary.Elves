; -- 辅助精灵安装脚本 --
#define MyAppName "Auxiliary Elves"
#define MyAppVersion "1.0.1"
#define MyAppPublisher "Auxiliary Elves"
#define MyAppExeName "Auxiliary.Elves.Client.exe"
#define SourcePath "D:\Workspace\Axu\Auxiliary.Elves.Client\bin\Release\net6.0-windows\"
#define IconPath "D:\Workspace\Axu\Auxiliary.Elves.Client\Assets\logo.ico"
#define DotNetInstaller "windowsdesktop-runtime-6.0.23-win-x64.exe"  ; 本地运行时安装包文件名
#define DotNetInstallerPath "D:\Workspace\Axu\" + DotNetInstaller  ; 运行时安装包完整路径

[Setup]
AppId={{3F13D2A1-8C4F-4A3B-9E6D-7B8C9A2B1C3D}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=output
OutputBaseFilename=AuxiliaryElvesSetup
Compression=lzma
SolidCompression=yes
SetupIconFile={#IconPath}
UninstallDisplayIcon={app}\{#MyAppExeName}
WizardStyle=modern
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "创建桌面图标(&D)"; GroupDescription: "附加图标:"; Flags: unchecked
Name: "desktopicon\english"; Description: "Create a &desktop icon"; GroupDescription: "Additional icons:"; Flags: unchecked

[Files]
; 打包 Release 目录下的所有内容和子目录
Source: "{#SourcePath}*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

; .NET 运行时安装包（打包到安装程序中）
Source: "{#DotNetInstallerPath}"; DestDir: "{tmp}"; Flags: deleteafterinstall

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\Uninstall {#MyAppName}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent

[Code]
// 检测.NET 6桌面运行时是否已安装
function IsDotNet60Installed: Boolean;
var
  success: Boolean;
  releaseVersion: Cardinal;
  version: string;
  installPath: string;
begin
  // 方法1: 检查注册表 - 新式.NET 6+ 安装
  success := RegQueryDWordValue(HKLM, 'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App', 'Version', releaseVersion);
  
  // 方法2: 检查具体的版本号
  if not success then
    success := RegKeyExists(HKLM, 'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App\6.0.23');
  
  // 方法3: 检查安装目录是否存在
  if not success then
  begin
    installPath := ExpandConstant('{sd}\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App\6.0.23\');
    success := DirExists(installPath);
  end;
  
  // 方法4: 检查文件是否存在
  if not success then
    success := FileExists(ExpandConstant('{sd}\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App\6.0.23\WindowsBase.dll'));
  
  // 方法5: 检查WOW6432Node注册表
  if not success then
    success := RegKeyExists(HKLM, 'SOFTWARE\WOW6432Node\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App');

  Result := success;
  
  // 调试信息（如果需要查看检测结果，可以取消注释）
  {
  if Result then
    MsgBox('.NET 6.0 Runtime is installed.', mbInformation, MB_OK)
  else
    MsgBox('.NET 6.0 Runtime is not installed.', mbInformation, MB_OK);
  }
end;

// 安装.NET运行时
function InstallDotNetRuntime: Boolean;
var
  ResultCode: Integer;
  InstallerPath: string;
begin
  Result := True;
  
  // 获取运行时安装包的路径
  InstallerPath := ExpandConstant('{tmp}\{#DotNetInstaller}');
  
  // 检查安装包是否存在
  if not FileExists(InstallerPath) then
  begin
    MsgBox('.NET Runtime installer not found in temporary directory.' + #13#10 +
           'Please make sure the file {#DotNetInstaller} is included in the setup.', 
           mbError, MB_OK);
    Result := False;
    Exit;
  end;
  
  // 显示安装提示
  if MsgBox('This application requires Microsoft .NET 6.0 Desktop Runtime.' + #13#10 +
            'The runtime will now be installed silently. This may take a few minutes.' + #13#10 +
            'Do you want to continue?', 
            mbConfirmation, MB_YESNO) = IDNO then
  begin
    Result := False;
    Exit;
  end;
  
  // 显示进度信息
  MsgBox('Installing .NET 6.0 Runtime...' + #13#10 +
         'Please wait, this may take several minutes.', 
         mbInformation, MB_OK);
  
  // 执行安装（静默安装，不显示界面）
  if Exec(InstallerPath, '/install /quiet /norestart', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
  begin
    // 检查安装结果
    case ResultCode of
      0: 
        begin
          MsgBox('.NET 6.0 Runtime installed successfully.', mbInformation, MB_OK);
          Result := True;
        end;
      3010: 
        begin
          MsgBox('.NET 6.0 Runtime installed successfully.' + #13#10 +
                 'A system restart is recommended for changes to take effect.', 
                 mbInformation, MB_OK);
          Result := True;
        end;
      else
        begin
          MsgBox('.NET Runtime installation failed with error code: ' + IntToStr(ResultCode) + #13#10 +
                 'Please install .NET 6.0 manually from Microsoft website.', 
                 mbError, MB_OK);
          Result := False;
        end;
    end;
  end
  else
  begin
    MsgBox('Failed to execute .NET Runtime installer.', mbError, MB_OK);
    Result := False;
  end;
end;

// 初始化安装
function InitializeSetup: Boolean;
var
  DotNetRequired: Boolean;
begin
  Result := True;
  
  // 检查.NET运行时是否已安装
  if not IsDotNet60Installed then
  begin
    // 尝试安装.NET运行时
    if not InstallDotNetRuntime then
    begin
      // 安装失败，询问是否继续
      if MsgBox('.NET Runtime installation failed or was cancelled.' + #13#10 +
                'The application may not work without .NET 6.0 Desktop Runtime.' + #13#10 +
                'Do you want to continue with the application installation?', 
                mbConfirmation, MB_YESNO) = IDNO then
      begin
        Result := False;
      end;
    end
    else
    begin
      // 安装成功，等待一下让系统注册表更新
      Sleep(2000);
    end;
  end;
end;

// 安装完成后最终检查
procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    // 最终验证.NET是否安装成功
    if not IsDotNet60Installed then
    begin
      MsgBox('Warning: .NET 6.0 Runtime may not be installed correctly.' + #13#10 +
             'If the application does not start, please manually install:' + #13#10 +
             'https://dotnet.microsoft.com/download/dotnet/6.0', 
             mbInformation, MB_OK);
    end;
  end;
end;