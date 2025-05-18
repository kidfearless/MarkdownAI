param (
    [switch]$UseNativeAot,
    [string]$Configuration = "Release",
    [string]$OutputPath = "publish",
    [switch]$InstallBuildTools
)

function Install-BuildTools {
    $buildToolsUrl = "https://aka.ms/vs/17/release/vs_buildtools.exe"
    $installerPath = Join-Path $env:TEMP "vs_buildtools.exe"
    
    Write-Host "Downloading Visual Studio Build Tools..." -ForegroundColor Cyan
    
    try {
        Invoke-WebRequest -Uri $buildToolsUrl -OutFile $installerPath
        
        if (Test-Path $installerPath) {
            Write-Host "Starting installer for Visual Studio Build Tools with C++ workload..." -ForegroundColor Green
            
            # Run the installer with required workloads for AOT compilation
            $installArgs = "--quiet", "--wait", "--norestart", "--nocache", 
                          "--installPath", "C:\BuildTools", 
                          "--add", "Microsoft.VisualStudio.Workload.VCTools", 
                          "--includeRecommended"
            
            $process = Start-Process -FilePath $installerPath -ArgumentList $installArgs -Wait -PassThru
            
            if ($process.ExitCode -eq 0 -or $process.ExitCode -eq 3010) {
                Write-Host "Visual Studio Build Tools installation completed successfully." -ForegroundColor Green
                if ($process.ExitCode -eq 3010) {
                    Write-Host "A system restart is required to complete the installation." -ForegroundColor Yellow
                }
                return $true
            }
            else {
                Write-Host "Installation failed with exit code: $($process.ExitCode)" -ForegroundColor Red
                return $false
            }
        }
        else {
            Write-Host "Failed to download the installer." -ForegroundColor Red
            return $false
        }
    }
    catch {
        Write-Host "Error occurred during installation: $_" -ForegroundColor Red
        return $false
    }
    finally {
        if (Test-Path $installerPath) {
            Remove-Item $installerPath -Force
        }
    }
}

$projectPath = Join-Path $PSScriptRoot "AITemplate\AITemplate.csproj"

# Check for Visual C++ build tools at common locations
$buildToolsPaths = @(
    "C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Auxiliary\Build\vcvars64.bat",
    "C:\Program Files\Microsoft Visual Studio\2022\BuildTools\VC\Auxiliary\Build\vcvars64.bat",
    "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\VC\Auxiliary\Build\vcvars64.bat",
    "C:\BuildTools\VC\Auxiliary\Build\vcvars64.bat"
)

$vcInstalled = $false
$vcVarsPath = $null

foreach ($path in $buildToolsPaths) {
    if (Test-Path $path) {
        $vcInstalled = $true
        $vcVarsPath = $path
        break
    }
}

# Install Build Tools if requested
if ($InstallBuildTools -and -not $vcInstalled) {
    Write-Host "Visual C++ build tools not found. Attempting to install..." -ForegroundColor Yellow
    $installSuccess = Install-BuildTools
    
    if ($installSuccess) {
        # Check again for the build tools after installation
        foreach ($path in $buildToolsPaths) {
            if (Test-Path $path) {
                $vcInstalled = $true
                $vcVarsPath = $path
                break
            }
        }
    }
}

# Determine if we can use Native AOT
if ($UseNativeAot) {
    if (-not $vcInstalled) {
        Write-Warning "Visual C++ build tools not found at expected locations."
        Write-Warning "Native AOT compilation requires Build Tools with Desktop Development with C++ workload."
        Write-Warning "Falling back to ReadyToRun compilation."
        Write-Host "Tip: Run this script with -InstallBuildTools to automatically download and install the required tools." -ForegroundColor Cyan
        $UseNativeAot = $false
    }
    else {
        Write-Host "Using Native AOT compilation for maximum performance." -ForegroundColor Green
        Write-Host "Using Visual C++ tools found at: $vcVarsPath" -ForegroundColor Gray
    }
}
else {
    Write-Host "Using ReadyToRun compilation." -ForegroundColor Yellow
    if ($vcInstalled) {
        Write-Host "Tip: Run with -UseNativeAot to enable full Native AOT compilation for better performance." -ForegroundColor Cyan
    }
    else {
        Write-Host "Tip: Run this script with -InstallBuildTools to install Visual C++ Build Tools." -ForegroundColor Cyan
    }
}

# Set the appropriate properties based on compilation mode
$extraParams = ""
if ($UseNativeAot) {
    $extraParams = "-p:NativeAot=true"
}

# Execute the publish command
$publishCommand = "dotnet publish `"$projectPath`" -c $Configuration -o `"$OutputPath`" $extraParams --self-contained"
Write-Host "Executing: $publishCommand" -ForegroundColor Gray
Invoke-Expression $publishCommand

if ($LASTEXITCODE -eq 0) {
    Write-Host "Successfully published to: $OutputPath" -ForegroundColor Green
}
else {
    Write-Host "Publish failed with exit code: $LASTEXITCODE" -ForegroundColor Red
    
    if ($UseNativeAot) {
        Write-Host "If Native AOT compilation failed, try installing the following:" -ForegroundColor Yellow
        Write-Host "1. Visual Studio with 'Desktop Development with C++' workload" -ForegroundColor Yellow
        Write-Host "2. For ARM64: C++ ARM64 build tools" -ForegroundColor Yellow
        Write-Host "3. Windows SDK components" -ForegroundColor Yellow
        Write-Host "See https://aka.ms/nativeaot-prerequisites for more details." -ForegroundColor Yellow
        
        Write-Host "Or run this script without the -UseNativeAot switch to use ReadyToRun compilation." -ForegroundColor Cyan
    }
}
