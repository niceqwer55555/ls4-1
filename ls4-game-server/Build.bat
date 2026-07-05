@echo off
chcp 65001 >nul
echo Building ls4-game-server...
cd /d "%~dp0"

set TOOLS_PATH=%~dp0..\Tools
set DOTNET_PATH=%TOOLS_PATH%\dotnet-sdk-10.0.103-win-x64

if not exist "%DOTNET_PATH%\dotnet.exe" (
    echo Error: dotnet.exe not found at: %DOTNET_PATH%
    pause
    exit /b 1
)

echo Using .NET SDK from: %DOTNET_PATH%
echo.

"%DOTNET_PATH%\dotnet.exe" build "Gameserver\GameServer.sln" --no-restore -c Release

if %errorlevel% equ 0 (
    echo.
    echo Build successful!
    echo.
    echo Copying output files...
    xcopy "Gameserver\GameServerConsole\bin\Release\net10.0\*" "server\" /Y /I /E
    if %errorlevel% equ 0 (
        echo Copy successful!
    ) else (
        echo Copy failed!
        pause
        exit /b 1
    )
) else (
    echo.
    echo Build failed!
    pause
    exit /b 1
)