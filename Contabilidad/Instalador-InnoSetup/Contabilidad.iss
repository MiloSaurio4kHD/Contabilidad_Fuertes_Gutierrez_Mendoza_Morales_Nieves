; Script de Inno Setup para el instalador de Contabilidad.
; Compilar con ISCC.exe (Inno Setup Compiler) para generar Contabilidad-Setup.exe.
; No requiere permisos de administrador: se instala en la carpeta del usuario actual.

#define MyAppName "Contabilidad"
#define MyAppVersion "1.0.2"
#define MyAppExeName "Contabilidad.exe"
#define MyReleaseDir "..\Contabilidad\bin\Release"

[Setup]
AppId={{8F1B2C3D-4E5A-4B6C-9D7E-1A2B3C4D5E6F}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher=LosPookemones
DefaultDirName={localappdata}\Programs\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
UninstallDisplayIcon={app}\{#MyAppExeName}
OutputDir=Output
OutputBaseFilename=Contabilidad-Setup
Compression=lzma
SolidCompression=yes
WizardStyle=modern
SetupIconFile=..\Contabilidad\Icons\iconoContabilidad.ico

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "desktopicon"; Description: "Crear un acceso directo en el Escritorio"; GroupDescription: "Accesos directos adicionales:"

[Files]
Source: "{#MyReleaseDir}\Contabilidad.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyReleaseDir}\Contabilidad.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyReleaseDir}\Data\*"; DestDir: "{app}\Data"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#MyReleaseDir}\Icons\*"; DestDir: "{app}\Icons"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\Desinstalar {#MyAppName}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Ejecutar {#MyAppName} ahora"; Flags: nowait postinstall skipifsilent

[Code]
function IsDotNet48OrLater: Boolean;
var
  Release: Cardinal;
begin
  Result := RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', Release) and (Release >= 528040);
end;

function InitializeSetup: Boolean;
begin
  Result := True;
  if not IsDotNet48OrLater then
  begin
    MsgBox('Este programa necesita .NET Framework 4.8 o superior. Windows 10 y 11 normalmente ya lo tienen instalado. Si la instalación o la ejecución fallan, descárgalo desde https://dotnet.microsoft.com/download/dotnet-framework antes de continuar.', mbInformation, MB_OK);
  end;
end;
