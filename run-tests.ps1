Write-Host "Running tests and generating reports..." -ForegroundColor Cyan

# Set timestamp for this test run
$timestamp = Get-Date -Format 'yyyyMMdd_HHmmss'
$reportDir = "TestReports\Run_$timestamp"

# Create report directory
if (-not (Test-Path $reportDir)) {
    New-Item -ItemType Directory -Path $reportDir | Out-Null
}

# Pass timestamp to test framework
$env:TEST_RUN_TIMESTAMP = $timestamp

# Run tests
dotnet test

# Generate Living Documentation
$testAssembly = "CoreTestFramework\bin\Debug\net8.0\CoreTestFramework.dll"
$testExecution = "CoreTestFramework\bin\Debug\net8.0\TestExecution.json"

if (Test-Path $testAssembly) {
    Write-Host "`nGenerating Living Documentation..." -ForegroundColor Cyan
    livingdoc test-assembly $testAssembly -t $testExecution -o "$reportDir\LivingDoc.html"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "`nTest reports generated successfully:" -ForegroundColor Green
        Write-Host "1. Test Diagnostics: $reportDir\TestDiagnostics.html"
        Write-Host "2. Living Documentation: $reportDir\LivingDoc.html"
    } else {
        Write-Host "`nFailed to generate Living Documentation" -ForegroundColor Red
    }
} else {
    Write-Host "`nTest assembly not found. Please build the solution first." -ForegroundColor Red
}

# Clean up environment variable
Remove-Item Env:\TEST_RUN_TIMESTAMP
