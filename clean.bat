@echo off
%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe src\geocoding.build /t:Clean /nologo
pause