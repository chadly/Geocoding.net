@echo off
%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe geocoding.build /t:Compile /nologo
pause