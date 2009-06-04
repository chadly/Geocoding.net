@echo off
%windir%\Microsoft.NET\Framework\v3.5\MSBuild.exe src\geocoding.build /t:Clean /nologo
pause