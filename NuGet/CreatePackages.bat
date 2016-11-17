@echo off

echo Create packages

set Version=%1
set Configuration=%2
set SolutionDir=..\

if not "%2" == "" goto createPackages

echo.
echo Usage: %0 [Version number: #.#.#] [Configuration: Release / Debug]
echo.

if "%1" == "" set /p Version=Version number [#.#.#]: 
if "%Version%" == "" goto exit

if "%2" == "" set /p Configuration=Configuration [Release / Debug]: 
if "%Configuration%" == "" goto exit

:createPackages
mkdir Output
nuget pack Granular.nuspec -Properties Version=%Version%;Configuration=%Configuration%;SolutionDir=%SolutionDir% -Output Output

if "%errorlevel%" == "9009" goto nugetIsMissing
goto exit

:nugetIsMissing
echo.
echo nuget.exe is missing, download it from
echo https://www.nuget.org/downloads
echo.
goto exit

:exit
pause
