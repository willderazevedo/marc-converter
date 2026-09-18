[Setup]
AppId={{328C9FF2-33F0-446F-BFFA-19917E5D4228}
AppName=Conversor MARC
AppVersion=1.0.0
AppPublisher=Willder Azevedo
AppPublisherURL=https://github.com/willderazevedo/marc-converter
AppSupportURL=https://github.com/willderazevedo/marc-converter
AppUpdatesURL=https://github.com/willderazevedo/marc-converter
DefaultDirName={autopf}\Conversor MARC
UninstallDisplayIcon={app}\marcc.exe
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
OutputBaseFilename=Conversor MARC
SolidCompression=yes
WizardStyle=modern dynamic

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "bin\Release\net10.0-windows\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "Resources\icon.ico"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\Conversor MARC"; Filename: "{app}\marcc.exe"
Name: "{autodesktop}\Conversor MARC"; Filename: "{app}\marcc.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\marcc.exe"; Description: "{cm:LaunchProgram,Conversor MARC}"; Flags: nowait postinstall skipifsilent