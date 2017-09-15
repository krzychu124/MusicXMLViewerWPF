# $msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"
$collectionOfArgs = "MusicXMLViewerWPF.sln"
#$msB = Resolve-Path "${env:ProgramFiles(x86)}\Microsoft Visual Studio\*\*\MSBuild\*\bin\MSBuild.exe"
$test = '"C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\bin\MSBuild.exe"'
& "C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\bin\MSBuild.exe" ./MusicXMLViewerWPF.sln
#& $test $collectionOfArgs