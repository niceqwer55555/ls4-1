@echo off
echo Building ls4-api-server...
cd /d "%~dp0"
mvn clean package

if %errorlevel% equ 0 (
    echo.
    echo Build successful!
    echo Copying jar to server directory...
    xcopy "target\*" "server\" /Y /I
    if %errorlevel% equ 0 (
        echo Copy successful!
    ) else (
        echo Copy failed!
        exit /b 1
    )
) else (
    echo.
    echo Build failed!
    exit /b 1
)