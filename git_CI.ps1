$msbuild = (Get-ItemProperty hklm:\software\Microsoft\MSBuild\ToolsVersions\4.0).MSBuildToolsPath
$MyDir = [System.IO.Path]::GetDirectoryName($myInvocation.MyCommand.Definition) 
. "$msbuild\msbuild.exe"