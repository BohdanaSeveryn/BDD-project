# Running and Managing Tests

## Requirements

- .NET 8.0 SDK or higher
- Visual Studio 2022 (recommended) or VS Code with C# extension

## Installing Dependencies

```bash
cd BookingSystem.Tests
dotnet restore
```

## Running All Tests

```bash
dotnet test
```

## Running Specific Test Sets

### By class name
```bash
dotnet test --filter "AuthenticationTests"
dotnet test --filter "BookingManagementTests"
```

### By method name
```bash
dotnet test --filter "SuccessfulAccountActivation"
```

### By User Story
```bash
dotnet test --filter "US-1"
dotnet test --filter "US-9"
```

## Detailed Output

```bash
dotnet test --verbosity detailed
```

Verbosity levels:
- `quiet` - no output
- `minimal` - errors only
- `normal` - standard output
- `detailed` - detailed information
- `diagnostic` - maximum information

## Running with Logging

```bash
dotnet test --logger "console;verbosity=detailed"
```

## Generating Code Coverage Report

```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura
```

## Running in Visual Studio

1. Open Solution in Visual Studio
2. Go to **Test** > **Run All Tests** (Ctrl + R, A)
3. Or use **Test Explorer** (Test > Test Explorer)

## Running in VS Code

1. Install **C#** extension from Microsoft
2. Open terminal (Ctrl + `)
3. Execute: `dotnet test`

## Filtering by DisplayName

```bash
dotnet test --filter "US-1: Successful activation"
```

## Running Single Test

```bash
dotnet test --filter "ClassName.MethodName"
```

Example:
```bash
dotnet test --filter "AuthenticationTests.SuccessfulAccountActivation_WhenValidActivationLinkAndPassword_AccountBecomesActive"
```

## Setting Up Tests for CI/CD

### GitHub Actions

Add file `.github/workflows/tests.yml`:

```yaml
name: Run Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      - run: dotnet restore
      - run: dotnet test --verbosity detailed
```

### Azure DevOps

```yaml
pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'

steps:
- task: UseDotNet@2
  inputs:
    version: '8.0.x'
- task: DotNetCoreCLI@2
  inputs:
    command: 'restore'
    projects: '**/*.csproj'
- task: DotNetCoreCLI@2
  inputs:
    command: 'test'
    projects: '**/*Tests.csproj'
```

## Debugging Tests

### In Visual Studio

1. Set breakpoint in test
2. Right-click on test in Test Explorer
3. Select "Debug Selected Tests"

### In VS Code

1. Set breakpoint
2. Execute command: `dotnet test --no-build --verbosity normal`
3. VS Code automatically attaches to debugger

## Current Test Status

### Number of Tests by Category

| Category | Number of Tests |
|----------|------------------|
| Authentication | 8 tests |
| Account Management | 13 tests |
| Booking Views | 10 tests |
| Time Slot Selection | 3 tests |
| Booking Confirmation | 10 tests |
| Booking List | 8 tests |
| Booking Cancellation | 4 tests |
| Administrative Operations | 13 tests |
| Notifications | 10 tests |
| **Total** | **~79 tests** |

## Short Commands (aliases)

Add to your shell:

### PowerShell
```powershell
Set-Alias -Name dt -Value 'dotnet test'
Set-Alias -Name dr -Value 'dotnet restore'
```

Usage:
```powershell
dt
dt --filter "US-1"
dr
```

### Bash
```bash
alias dt='dotnet test'
alias dr='dotnet restore'
```

## Syntax and Format Checking

```bash
# Build project (without running tests)
dotnet build

# Clean old builds
dotnet clean
```

## Test Execution Checklist

- [ ] .NET 8.0 SDK installed
- [ ] Repository cloned
- [ ] `dotnet restore` executed
- [ ] `dotnet test` executed (all tests should pass)
- [ ] Set up in VS or VS Code
- [ ] Understand test results

## Troubleshooting

### Issue: Tests Not Found
```bash
# Verify you are in the correct directory
cd BookingSystem.Tests

# Rebuild the project
dotnet clean
dotnet build
```

### Issue: Dependency Error
```bash
# Update NuGet packages
dotnet nuget update source
dotnet restore
```

### Issue: Tests are hanging
```bash
# Use timeout
dotnet test --configuration Release --no-build -- RunConfiguration.TestSessionTimeout=60000
```

## Useful Links

- [xUnit documentation](https://xunit.net/)
- [Moq documentation](https://github.com/moq/moq4/wiki/Quickstart)
- [FluentAssertions](https://fluentassertions.com/)
- [.NET testing](https://learn.microsoft.com/en-us/dotnet/core/testing/)
