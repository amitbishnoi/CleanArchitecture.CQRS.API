# Running Tests - Quick Start Guide

## Prerequisites

- .NET 8 SDK installed
- xUnit test framework
- Moq mocking library
- FluentAssertions library

All dependencies are already configured in `Tests.xUnit.csproj`

---

## Quick Commands

### 1. Run All Tests
```powershell
cd .\Tests.xUnit
dotnet test
```

**Output Example**:
```
Test Run Successful.
Total tests: 37
Passed: 37
Failed: 0
Skipped: 0
Time: 2.345s
```

---

### 2. Run Specific Test Class
```powershell
dotnet test --filter "FullyQualifiedName~CreateUserCommandHandlerTests"
```

---

### 3. Run Specific Test Method
```powershell
dotnet test --filter "Name=Handle_WithValidCommand_CreatesUserSuccessfully"
```

---

### 4. Run Tests by Category
```powershell
# Authentication tests
dotnet test --filter "FullyQualifiedName~Authentication"

# User tests
dotnet test --filter "FullyQualifiedName~Users"

# Course tests
dotnet test --filter "FullyQualifiedName~Courses"

# Service tests
dotnet test --filter "FullyQualifiedName~Services"

# Integration tests
dotnet test --filter "FullyQualifiedName~Integration"
```

---

### 5. Run with Detailed Output
```powershell
dotnet test --logger "console;verbosity=detailed"
```

---

### 6. Run Tests and Generate Report
```powershell
dotnet test --logger "trx" --collect:"XPlat Code Coverage"
```

Report will be generated in:
```
TestResults/
├── [guid].trx (Test results)
└── [guid]/coverage.cobertura.xml (Coverage report)
```

---

### 7. Run Tests in Watch Mode (Auto-rerun on file changes)
```powershell
dotnet watch test
```

---

## Test Categories

### Unit Tests (37 tests total)

**Authentication** (1 test)
- LoginCommandHandler tests

**Users** (4 tests)
- CreateUserCommandHandler tests
- GetAllUsersQueryHandler tests

**Courses** (4 tests)
- CreateCourseCommandHandler tests
- GetAllCoursesQueryHandler tests

**Enrollment** (3 tests)
- CreateEnrollmentCommandHandler tests

**Services** (12 tests)
- CacheService tests (5)
- PasswordHasher tests (7)

**Common Models** (5 tests)
- PaginationParams tests

**Integration Tests** (10 tests)
- UserRepository integration tests (5)
- CourseRepository integration tests (5)

---

## Understanding Test Results

### Passed Test ✅
```
PASSED Tests.xUnit.Features.Users.CreateUserCommandHandlerTests.
       Handle_WithValidCommand_CreatesUserSuccessfully
       [19 ms]
```

### Failed Test ❌
```
FAILED Tests.xUnit.Features.Users.CreateUserCommandHandlerTests.
       Handle_WithInvalidEmail_ValidationShouldFail

Error: Expected 0 validation errors but found 1
```

### Skipped Test ⊘
```
SKIPPED Tests.xUnit.Features.Users.CreateUserCommandHandlerTests.
        Handle_WithPendingImplementation
```

---

## Common Issues & Solutions

### Issue 1: "dotnet test" command not found
**Solution:**
```powershell
# Ensure you're in the correct directory
cd .\Tests.xUnit

# Or run from project root with project path
dotnet test .\Tests.xUnit\Tests.xUnit.csproj
```

---

### Issue 2: Test timeout
**Solution:**
```powershell
# Increase timeout (default is 30 seconds)
dotnet test --logger "console;verbosity=detailed" --configuration Release
```

---

### Issue 3: In-memory database error
**Solution**: Each integration test creates its own unique database:
```csharp
var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
    .Options;
```

This ensures test isolation automatically.

---

## GitHub Actions Integration

Add to `.github/workflows/tests.yml`:

```yaml
name: Run Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Run tests
      run: dotnet test Tests.xUnit/Tests.xUnit.csproj --verbosity normal
```

---

## Test Naming Convention

All tests follow the pattern:
```
[MethodName]_[Scenario]_[ExpectedResult]
```

Examples:
- `Handle_WithValidCommand_CreatesUserSuccessfully`
- `HashPassword_WithSamePassword_ProducesDifferentHashes`
- `RemoveByPattern_WithMatchingPattern_RemovesMatching`

---

## Adding New Tests

### 1. Create a new test file in appropriate folder:
```csharp
public class MyNewHandlerTests
{
    private readonly Mock<IService> _mockService;
    private readonly MyNewHandler _handler;
    
    public MyNewHandlerTests()
    {
        _mockService = new Mock<IService>();
        _handler = new MyNewHandler(_mockService.Object);
    }
    
    [Fact]
    public async Task Handle_WithValidCommand_ReturnsExpectedResult()
    {
        // Arrange
        var command = new MyCommand { /* ... */ };
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
    }
}
```

### 2. Run the new test:
```powershell
dotnet test --filter "Name=Handle_WithValidCommand_ReturnsExpectedResult"
```

---

## Useful Test Debugging Tips

### 1. Add Debug Output
```csharp
[Fact]
public async Task MyTest()
{
    var result = await _handler.Handle(command, CancellationToken.None);
    Console.WriteLine($"Result: {result}");
    result.Should().BeGreaterThan(0);
}
```

Run with:
```powershell
dotnet test --logger "console;verbosity=detailed"
```

### 2. Pause on Failure
```csharp
[Fact]
public async Task MyTest()
{
    var result = await _handler.Handle(command, CancellationToken.None);
    
    if (result != expected)
    {
        System.Diagnostics.Debugger.Break(); // Pauses debugger
    }
    
    result.Should().Be(expected);
}
```

### 3. Use Theory for Multiple Scenarios
```csharp
[Theory]
[InlineData(10)]
[InlineData(25)]
[InlineData(50)]
public void PaginationParams_WithVariousPageSizes_Validates(int pageSize)
{
    var pagination = new PaginationParams { PageSize = pageSize };
    pagination.PageSize.Should().BeLessThanOrEqualTo(50);
}
```

---

## Performance Tips

- **Run tests in Release mode for faster execution**:
  ```powershell
  dotnet test --configuration Release
  ```

- **Run tests in parallel** (default behavior)

- **Use `--no-build` to skip rebuild**:
  ```powershell
  dotnet build && dotnet test --no-build
  ```

---

## Code Coverage Analysis

### Generate coverage report:
```powershell
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

### View coverage (requires ReportGenerator):
```powershell
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"coverage.opencover.xml" -targetdir:"coverage-report"
```

Then open `coverage-report/index.html` in browser.

---

## Continuous Testing

Use `dotnet watch` for automatic test re-run:

```powershell
dotnet watch test
```

This will re-run all tests whenever any source file changes. Perfect for TDD!

---

For detailed information about test cases, see: `TEST_CASES.md`
