$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
$project = "tests/OOP_3.Tests.csproj"
dotnet restore $project
if ($LASTEXITCODE -ne 0) {
    throw "NuGet restore failed"
}

$undetected = @()
$mutations = @(
    @{ Path = "src/Employee.cs"; Before = "hours > 60"; After = "hours > 61"; Name = "hours boundary" },
    @{ Path = "src/SalariedEmployee.cs"; Before = "if (salary < MIN_SALARY)"; After = "if (salary < MIN_SALARY && false)"; Name = "minimum salary" },
    @{ Path = "src/EmployeeWorkflowService.cs"; Before = "employee.CalculatePayment() * years * annualPercent / 100.0"; After = "employee.CalculatePayment() * years * annualPercent * 100.0"; Name = "seniority bonus" },
    @{ Path = "src/Department.cs"; Before = "return TotalPayroll() / _count;"; After = "return TotalPayroll() / employees.Length;"; Name = "average payment" },
    @{ Path = "src/BonusPolicies.cs"; Before = "basePayment * _percent / 100.0"; After = "0"; Name = "percentage bonus" }
)

foreach ($mutation in $mutations) {
    $path = $mutation.Path
    $original = [System.IO.File]::ReadAllText($path)
    if (-not $original.Contains($mutation.Before)) {
        throw "Mutation source text not found: $($mutation.Name)"
    }

    try {
        $mutated = $original.Replace($mutation.Before, $mutation.After)
        [System.IO.File]::WriteAllText($path, $mutated, [System.Text.UTF8Encoding]::new($false))
        Write-Host "Mutation: $($mutation.Name)"
        dotnet test $project --no-restore
        $testExitCode = $LASTEXITCODE
        if ($testExitCode -eq 0) {
            Write-Host "NOT DETECTED: $($mutation.Name)" -ForegroundColor Red
            $undetected += $mutation.Name
        }
        else {
            Write-Host "DETECTED: $($mutation.Name)" -ForegroundColor Green
        }
    }
    finally {
        [System.IO.File]::WriteAllText($path, $original, [System.Text.UTF8Encoding]::new($false))
    }
}

Write-Host "All source files have been restored."
Pop-Location
if ($undetected.Count -gt 0) {
    throw "Mutations were not detected: $($undetected -join ", ")"
}
