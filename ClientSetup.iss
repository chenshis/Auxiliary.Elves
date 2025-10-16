; -- �������鰲װ�ű� --
#define MyAppName "��������"
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
; �������ļ�
Source: "{#SourcePath}*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs
; .NET 6 ����ʱ��װ����
Source: "{#DotNetRuntimeInstaller}"; DestDir: "{tmp}"; Flags: ignoreversion deleteafterinstall solidbreak
Source: "{#DotNetAspNetCoreRuntime}"; DestDir: "{tmp}"; Flags: ignoreversion deleteafterinstall solidbreak

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
; ��װ .NET 6 ��������ʱ�������Ҫ��
Filename: "{tmp}\{#DotNetRuntimeInstaller}"; \
    Parameters: "/install /quiet /norestart"; \
    StatusMsg: "���ڰ�װ .NET 6 ��������ʱ..."; \
    Check: IsDotNetDesktopRuntimeNeeded; \
    Flags: waituntilterminated

; ��װ ASP.NET Core ����ʱ�������Ҫ��
Filename: "{tmp}\{#DotNetAspNetCoreRuntime}"; \
    Parameters: "/install /quiet /norestart"; \
    StatusMsg: "���ڰ�װ ASP.NET Core ����ʱ..."; \
    Check: IsAspNetCoreRuntimeNeeded; \
    Flags: waituntilterminated

; ����Ӧ�ó���
Filename: "{app}\{#MyAppExeName}"; \
    Description: "���� {#MyAppName}"; \
    Flags: nowait postinstall skipifsilent

[UninstallRun]
Filename: "{sys}\taskkill.exe"; Parameters: "/f /im {#MyAppExeName}"; Flags: runhidden waituntilterminated

[Code]
function IsDotNetDesktopRuntimeNeeded: Boolean;
var
  Version: string;
  Install: Boolean;
begin
  // ����Ƿ��Ѱ�װ .NET 6 ��������ʱ
  Install := True;
  
  // ����1: ���ע���
  if RegQueryStringValue(HKLM, 'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App', '6.0.36', Version) then
  begin
    if Version <> '' then
      Install := False;
  end;
  
  // ����2: ����������ܵ�ע���·��
  if Install and RegKeyExists(HKLM, 'SOFTWARE\WOW6432Node\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App\6.0.36') then
    Install := False;
  
  // ����3: ����ļ��Ƿ����
  if Install and FileExists(ExpandConstant('{sys}\dotnet\shared\Microsoft.WindowsDesktop.App\6.0.36\WpfGfx.exe')) then
    Install := False;
  
  Result := Install;
  
  if Result then
    Log('.NET Desktop Runtime ��Ҫ��װ')
  else
    Log('.NET Desktop Runtime �Ѱ�װ��������װ');
end;

function IsAspNetCoreRuntimeNeeded: Boolean;
var
  Version: string;
  Install: Boolean;
begin
  // ����Ƿ��Ѱ�װ ASP.NET Core 6 ����ʱ
  Install := True;
  
  // ����1: ���ע���
  if RegQueryStringValue(HKLM, 'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.AspNetCore.App', '6.0.36', Version) then
  begin
    if Version <> '' then
      Install := False;
  end;
  
  // ����2: ����������ܵ�ע���·��
  if Install and RegKeyExists(HKLM, 'SOFTWARE\WOW6432Node\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.AspNetCore.App\6.0.36') then
    Install := False;
  
  // ����3: ����ļ��Ƿ����
  if Install and FileExists(ExpandConstant('{sys}\dotnet\shared\Microsoft.AspNetCore.App\6.0.36\Microsoft.AspNetCore.dll')) then
    Install := False;
  
  Result := Install;
  
  if Result then
    Log('ASP.NET Core Runtime ��Ҫ��װ')
  else
    Log('ASP.NET Core Runtime �Ѱ�װ��������װ');
end;

function InitializeSetup: Boolean;
begin
  // ��ʼ�����
  Result := True;
  
  // ����Ƿ�64λϵͳ
  if not Is64BitInstallMode then
  begin
    MsgBox('��Ӧ�ó�����Ҫ64λWindows����ϵͳ��', mbError, MB_OK);
    Result := False;
    Exit;
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  case CurStep of
    ssInstall:
      begin
        // ��װ��ʼǰ��¼
        Log('��ʼ��װ����');
        
        // ��¼����ʱ��װ״̬
        if IsDotNetDesktopRuntimeNeeded then
          Log('��Ҫ��װ .NET Desktop Runtime')
        else
          Log('.NET Desktop Runtime �Ѵ���');
          
        if IsAspNetCoreRuntimeNeeded then
          Log('��Ҫ��װ ASP.NET Core Runtime')
        else
          Log('ASP.NET Core Runtime �Ѵ���');
      end;
    ssPostInstall:
      begin
        // ��װ����
        Log('��װ�������');
      end;
  end;
end;

function InitializeUninstall: Boolean;
begin
  // ж��ǰ�������Ƿ�������
  if CheckForMutexes('AuxiliaryElves') or CheckForMutexes('Auxiliary.Elves.Client') then
  begin
    if MsgBox('��⵽Ӧ�ó����������С��Ƿ����ж�أ�', mbConfirmation, MB_YESNO) = IDNO then
      Result := False
    else
      Result := True;
  end
  else
    Result := True;
end;

// ���Ӧ�ó����Ƿ������е��������
function IsProcessRunning(const ExeFileName: string): Boolean;
var
  ResultCode: Integer;
begin
  // ʹ�������й��߼�����
  Exec('cmd.exe', '/C tasklist /FI "IMAGENAME eq ' + ExeFileName + '" | find /I "' + ExeFileName + '" > nul', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  Result := (ResultCode = 0); // �ҵ����̷���0
end;
