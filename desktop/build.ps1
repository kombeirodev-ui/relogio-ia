param([switch]$Test)
$ErrorActionPreference = 'Stop'
$taskRoot = Split-Path $PSScriptRoot -Parent
$taskOut = Join-Path $taskRoot 'desktop\release'
New-Item -ItemType Directory -Path $taskOut -Force | Out-Null
$taskCompiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
$taskSources = @('Core.cs','SpotifyMedia.cs','SpotifyVolume.cs','VolumeSlider.cs','MusicPanel.cs','Sensors.cs','GpuReader.cs','WindowPlacement.cs','SoundAlerts.cs','CodexClient.cs','ClaudeClient.cs','Themes.cs','BasicSkin.cs','PublicArt.cs','WinampSkin.cs','BrandArt.cs','DesktopSkins.cs','RetroSkins.cs','AeroIcons.cs','AeroSkin.cs','XpSkin.cs','EmbeddedFonts.cs','SettingsForm.cs','RelogioIA.cs') | ForEach-Object { Join-Path $PSScriptRoot $_ }
$taskResources = @(
    "/resource:$PSScriptRoot\fonts\IBMPlexSans-Regular.ttf,PlexSansRegular"
    "/resource:$PSScriptRoot\fonts\IBMPlexSans-Bold.ttf,PlexSansBold"
    "/resource:$PSScriptRoot\fonts\IBMPlexMono-Regular.ttf,PlexMono"
    "/resource:$PSScriptRoot\fonts\Inter-Regular.ttf,InterRegular"
    "/resource:$PSScriptRoot\fonts\Inter-Bold.ttf,InterBold"
    "/resource:$PSScriptRoot\fonts\Roboto.ttf,RobotoRegular"
    "/resource:$PSScriptRoot\assets\otimizepc-symbol.png,OtimizeSymbol"
    "/resource:$PSScriptRoot\assets\aero-aquatico.png,AeroAquatic"
)
$taskWinmd = Get-ChildItem 'C:\Program Files (x86)\Windows Kits\10\UnionMetadata\*\Windows.winmd' | Where-Object { $_.Directory.Name -match '^10\.0\.' } | Sort-Object { [version]$_.Directory.Name } -Descending | Select-Object -First 1
if (-not $taskWinmd) { throw 'Instale o Windows SDK para compilar a integração de mídia.' }
$taskRuntime = Get-ChildItem "$env:WINDIR\Microsoft.NET\assembly\GAC_MSIL\System.Runtime\*\System.Runtime.dll" | Select-Object -First 1
& $taskCompiler /nologo /reference:$($taskWinmd.FullName) /reference:$($taskRuntime.FullName) /reference:System.Runtime.WindowsRuntime.dll /reference:System.Runtime.InteropServices.WindowsRuntime.dll /target:winexe /optimize+ /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:System.Web.Extensions.dll /out:"$taskOut\RelogioIA.exe" @taskResources @taskSources
if ($LASTEXITCODE -ne 0) { throw 'Falha ao compilar o relógio.' }
Get-Item -LiteralPath "$taskOut\RelogioIA.exe" | Select-Object FullName, Length
if ($Test) {
    & $taskCompiler /nologo /reference:System.Drawing.dll /out:"$taskOut\Tests.exe" "$PSScriptRoot\Tests.cs"
    if ($LASTEXITCODE -ne 0) { throw 'Falha ao compilar testes.' }
    & "$taskOut\Tests.exe" "$taskOut\RelogioIA.exe"
    if ($LASTEXITCODE -ne 0) { throw 'Falha nos testes do desktop.' }
}
