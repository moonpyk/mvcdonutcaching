Param(
   [string]$Version = ''
)
$scriptRoot = Split-Path (Resolve-Path $myInvocation.MyCommand.Path) 
$OutputDirectory = "$scriptRoot\nuget\"

Set-Alias Build-Pkg-Internal $scriptRoot\.nuget\NuGet.exe

Build-Pkg-Internal pack "$($scriptRoot)\DevTrends.MvcDonutCaching\MvcDonutCaching.csproj" -OutputDirectory $OutputDirectory  -Prop Version=$Version -Symbols -Verbose