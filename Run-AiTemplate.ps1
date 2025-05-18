# PowerShell script to run the AITemplate application

function Show-Usage {
    Write-Host "AI Template Runner" -ForegroundColor Cyan
    Write-Host "------------------" -ForegroundColor Cyan
    Write-Host
    Write-Host "Usage: .\Run-AiTemplate.ps1 [file-path] [options]" -ForegroundColor Yellow
    Write-Host
    Write-Host "Options:"
    Write-Host "  --no-update    Do not update the original file with the AI response"
    Write-Host
    Write-Host "Examples:"
    Write-Host "  .\Run-AiTemplate.ps1 `"c:\path\to\your-file.mdai`"" -ForegroundColor Yellow
    Write-Host "  .\Run-AiTemplate.ps1 `"c:\path\to\your-file.mdai`" --no-update" -ForegroundColor Yellow
    Write-Host
}

# Check if at least one argument is provided
if ($args.Count -eq 0) {
    Show-Usage
    exit
}

# Get the script directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$appPath = Join-Path $scriptDir "AITemplate\bin\Debug\net9.0\win-x64\AITemplate.dll"

# Check if the application exists
if (-not (Test-Path $appPath)) {
    Write-Host "Error: AITemplate application not found at $appPath" -ForegroundColor Red
    Write-Host "Please build the project first with 'dotnet build'" -ForegroundColor Red
    exit 1
}

# Run the application with the provided arguments
Write-Host "AI Template Runner" -ForegroundColor Cyan
Write-Host "------------------" -ForegroundColor Cyan
Write-Host

try {
    & dotnet $appPath $args
    Write-Host
    Write-Host "Done!" -ForegroundColor Green
} catch {
    Write-Host "Error running AITemplate: $_" -ForegroundColor Red
    exit 1
}
