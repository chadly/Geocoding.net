@echo off
%windir%\Microsoft.NET\Framework\v3.5\MSBuild.exe src\geocoding.build /p:Configuration=Release /nologo
pause