$msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"
Invoke-Command { $msbuild "MusicXMLViewerWPF.sln" }