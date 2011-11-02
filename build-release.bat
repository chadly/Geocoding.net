@echo off
%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe geocoding.build /p:Configuration=Release /nologo
pause