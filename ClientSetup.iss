; -- 辅助精灵安装脚本 --
#define MyAppName "辅助精灵"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Your Company"
#define MyAppExeName "Auxiliary.Elves.Client.exe"
#define SourcePath "Auxiliary.Elves.Client\bin\Release\net6.0-windows\"
#define IconPath "Auxiliary.Elves.Client\Assets\logo.ico"
#define DotNetRuntimeInstaller "windowsdesktop-runtime-6.0.36-win-x64.exe"
#define DotNetAspNetCoreRuntime "aspnetcore-runtime-6.0.36-win-x64.exe"

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
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkedonce
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 6.01

[Files]
; 主程序文件
Source: "{#SourcePath}*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs
; .NET 6 运行时安装程序
Source: "{#DotNetRuntimeInstaller}"; DestDir: "{tmp}"; Flags: ignoreversion deleteafterinstall solidbreak
Source: "{#DotNetAspNetCoreRuntime}"; DestDir: "{tmp}"; Flags: ignoreversion deleteafterinstall solidbreak

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
; 安装 .NET 6 桌面运行时（如果需要）
Filename: "{tmp}\{#DotNetRuntimeInstaller}"; \
    Parameters: "/install /quiet /norestart"; \
    StatusMsg: "正在安装 .NET 6 桌面运行时..."; \
    Check: IsDotNetDesktopRuntimeNeeded; \
    Flags: waituntilterminated

; 安装 ASP.NET Core 运行时（如果需要）
Filename: "{tmp}\{#DotNetAspNetCoreRuntime}"; \
    Parameters: "/install /quiet /norestart"; \
    StatusMsg: "正在安装 ASP.NET Core 运行时..."; \
    Check: IsAspNetCoreRuntimeNeeded; \
    Flags: waituntilterminated

; 启动应用程序
Filename: "{app}\{#MyAppExeName}"; \
    Description: "启动 {#MyAppName}"; \
    Flags: nowait postinstall skipifsilent

[UninstallRun]
Filename: "{sys}\taskkill.exe"; Parameters: "/f /im {#MyAppExeName}"; Flags: runhidden waituntilterminated

[Code]
function IsDotNetDesktopRuntimeNeeded: Boolean;
var
  Version: string;
  Install: Boolean;
begin
  // 检查是否已安装 .NET 6 桌面运行时
  Install := True;
  
  // 方法1: 检查注册表
  if RegQueryStringValue(HKLM, 'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App', '6.0.36', Version) then
  begin
    if Version <> '' then
      Install := False;
  end;
  
  // 方法2: 检查其他可能的注册表路径
  if Install and RegKeyExists(HKLM, 'SOFTWARE\WOW6432Node\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App\6.0.36') then
    Install := False;
  
  // 方法3: 检查文件是否存在
  if Install and FileExists(ExpandConstant('{sys}\dotnet\shared\Microsoft.WindowsDesktop.App\6.0.36\WpfGfx.exe')) then
    Install := False;
  
  Result := Install;
  
  if Result then
    Log('.NET Desktop Runtime 需要安装')
  else
    Log('.NET Desktop Runtime 已安装，跳过安装');
end;

function IsAspNetCoreRuntimeNeeded: Boolean;
var
  Version: string;
  Install: Boolean;
begin
  // 检查是否已安装 ASP.NET Core 6 运行时
  Install := True;
  
  // 方法1: 检查注册表
  if RegQueryStringValue(HKLM, 'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.AspNetCore.App', '6.0.36', Version) then
  begin
    if Version <> '' then
      Install := False;
  end;
  
  // 方法2: 检查其他可能的注册表路径
  if Install and RegKeyExists(HKLM, 'SOFTWARE\WOW6432Node\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.AspNetCore.App\6.0.36') then
    Install := False;
  
  // 方法3: 检查文件是否存在
  if Install and FileExists(ExpandConstant('{sys}\dotnet\shared\Microsoft.AspNetCore.App\6.0.36\Microsoft.AspNetCore.dll')) then
    Install := False;
  
  Result := Install;
  
  if Result then
    Log('ASP.NET Core Runtime 需要安装')
  else
    Log('ASP.NET Core Runtime 已安装，跳过安装');
end;

function InitializeSetup: Boolean;
begin
  // 初始化检查
  Result := True;
  
  // 检查是否64位系统
  if not Is64BitInstallMode then
  begin
    MsgBox('此应用程序需要64位Windows操作系统。', mbError, MB_OK);
    Result := False;
    Exit;
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  case CurStep of
    ssInstall:
      begin
        // 安装开始前记录
        Log('开始安装过程');
        
        // 记录运行时安装状态
        if IsDotNetDesktopRuntimeNeeded then
          Log('需要安装 .NET Desktop Runtime')
        else
          Log('.NET Desktop Runtime 已存在');
          
        if IsAspNetCoreRuntimeNeeded then
          Log('需要安装 ASP.NET Core Runtime')
        else
          Log('ASP.NET Core Runtime 已存在');
      end;
    ssPostInstall:
      begin
        // 安装后处理
        Log('安装步骤完成');
      end;
  end;
end;

function InitializeUninstall: Boolean;
begin
  // 卸载前检查程序是否在运行
  if CheckForMutexes('AuxiliaryElves') or CheckForMutexes('Auxiliary.Elves.Client') then
  begin
    if MsgBox('检测到应用程序正在运行。是否继续卸载？', mbConfirmation, MB_YESNO) = IDNO then
      Result := False
    else
      Result := True;
  end
  else
    Result := True;
end;

// 检查应用程序是否在运行的替代方法
function IsProcessRunning(const ExeFileName: string): Boolean;
var
  ResultCode: Integer;
begin
  // 使用命令行工具检查进程
  Exec('cmd.exe', '/C tasklist /FI "IMAGENAME eq ' + ExeFileName + '" | find /I "' + ExeFileName + '" > nul', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  Result := (ResultCode = 0); // 找到进程返回0
end;
