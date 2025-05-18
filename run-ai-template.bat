@echo off
REM This batch file is a helper to run the AITemplate application on Windows

echo AI Template Runner
echo ------------------
echo.

IF "%~1"=="" (
    echo Usage: run-ai-template.bat [file-path] [options]
    echo.
    echo Options:
    echo   --no-update    Do not update the original file with the AI response
    echo.
    echo Examples:
    echo   run-ai-template.bat "c:\path\to\your-file.mdai"
    echo   run-ai-template.bat "c:\path\to\your-file.mdai" --no-update
    echo.
    goto :EOF
)

REM Run the application with the provided arguments
dotnet "%~dp0AITemplate\bin\Debug\net9.0\win-x64\AITemplate.dll" %*

echo.
echo Done!
