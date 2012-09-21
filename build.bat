@echo off
%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe src\nuget.build /nologo
%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe src\framework.build /nologo
pause