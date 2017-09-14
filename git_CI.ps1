# $msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"
$collectionOfArgs = @("MusicXMLViewerWPF.sln")
Invoke-Expression $msbuild $collectionOfArgs