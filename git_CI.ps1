# $msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"
$collectionOfArgs = "MusicXMLViewerWPF.sln"
$msB = Resolve-Path "${env:ProgramFiles(x86)}\Microsoft Visual Studio\*\*\MSBuild\*\bin\MSBuild.exe"
& $msB[0] $collectionOfArgs