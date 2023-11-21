@echo off

SET %publish_folder=_Published

IF "%1"=="spa" (call :BUILD_SPA)
IF "%2"=="spa" (call :BUILD_SPA)
IF "%1"=="dotnet" (call :BUILD_DOTNET)
IF "%2"=="dotnet" (call :BUILD_DOTNET)
exit

:BUILD_SPA
cd AngularTemplate.SPA
SET wwwroot=..\%publish_folder%\wwwroot
call ng build --output-path %wwwroot%
cd ..
exit /b

:BUILD_DOTNET
dotnet publish AngularTemplate.Web\AngularTemplate.Web.csproj -c Release -f net6.0 -o %publish_folder%
exit /b