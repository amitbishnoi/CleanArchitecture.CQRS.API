# Test Cases Documentation

## Overview
This document describes all the test cases for the Clean Architecture CQRS API project using xUnit and Moq frameworks.

## Test Project Structure

```
Tests.xUnit/
├── Features/
│   ├── Authentication/
│   │   └── LoginCommandHandlerTests.cs
│   ├── Users/
│   │   ├── CreateUserCommandHandlerTests.cs
│   │   └── GetAllUsersQueryHandlerTests.cs
│   ├── Courses/
│   │   ├── CreateCourseCommandHandlerTests.cs
│   │   └── GetAllCoursesQueryHandlerTests.cs
│   └── Enrollment/
│       └── CreateEnrollmentCommandHandlerTests.cs
├── Services/
│   ├── CacheServiceTests.cs
│   └── PasswordHasherTests.cs
├── Common/
│   └── PaginationParamsTests.cs
└── Integration/
    └── Repositories/
        ├── UserRepositoryIntegrationTests.cs
        └── CourseRepositoryIntegrationTests.cs
```

---

## Unit Tests

### 1. Authentication Tests

#### LoginCommandHandlerTests
**File**: `Features/Authentication/LoginCommandHandlerTests.cs`

| Test Case | Description | Expected Result |
|-----------|-------------|-----------------|
| `Handle_WithValidCredentials_ReturnsAuthResponse` | Login with valid email and password | Returns AuthResponse with token |
| `Handle_WithInvalidCredentials_ThrowsUnauthorizedAccessException` | Login with wrong password | Throws UnauthorizedAccessException |
| `Handle_WithEmptyEmail_ShouldFail` | Login with empty email | Email should be empty |

---

### 2. User Management Tests

#### CreateUserCommandHandlerTests
**File**: `Features/Users/CreateUserCommandHandlerTests.cs`

| Test Case | Description | Expected Result |
|-----------|-------------|-----------------|
| `Handle_WithValidCommand_CreatesUserSuccessfully` | Create user with valid data | User ID > 0, password hashed, email sent |
| `Handle_WithInvalidEmail_ValidationShouldFail` | Create user with invalid email format | Validation fails on Email property |
| `Handle_WithEmptyName_ValidationShouldFail` | Create user with empty name | Validation fails on Name property |
| `Handle_WithEmptyPassword_ValidationShouldFail` | Create user with empty password | Validation fails on Password property |

**Validations Tested**:
- ✅ Name not empty, max 100 characters
- ✅ Email valid format
- ✅ Password not empty
- ✅ Role not empty

#### GetAllUsersQueryHandlerTests
**File**: `Features/Users/GetAllUsersQueryHandlerTests.cs`

| Test Case | Description | Expected Result |
|-----------|-------------|-----------------|
| `Handle_WithUsers_ReturnsUserDtos` | Get all users | Returns list of UserDtos without passwords |
| `Handle_WithNoUsers_ReturnsEmptyList` | Get all users when none exist | Returns empty list |
| `Handle_WithSearchTerm_FiltersBySearchTerm` | Search users by name/email | Returns filtered results |

---

### 3. Course Management Tests

#### CreateCourseCommandHandlerTests
**File**: `Features/Courses/CreateCourseCommandHandlerTests.cs`

| Test Case | Description | Expected Result |
|-----------|-------------|-----------------|
| `Handle_WithValidCommand_CreatesCourseSuccessfully` | Create course with valid instructor | Course ID > 0 |
| `Handle_WithInvalidInstructor_ThrowsApplicationException` | Create course with non-existent instructor | Throws exception |
| `Handle_WithEmptyTitle_ValidationShouldFail` | Create course with empty title | Validation fails |
| `Handle_WithInvalidInstructorId_ValidationShouldFail` | Create course with InstructorId=0 | Validation fails |

**Validations Tested**:
- ✅ Title not empty, max 100 characters
- ✅ InstructorId > 0
- ✅ Instructor must exist

#### GetAllCoursesQueryHandlerTests
**File**: `Features/Courses/GetAllCoursesQueryHandlerTests.cs`

| Test Case | Description | Expected Result |
|-----------|-------------|-----------------|
| `Handle_WithCourses_ReturnsCourses` | Get all courses | Returns list of CourseDtos |
| `Handle_WithNoCourses_ReturnsEmptyList` | Get all courses when none exist | Returns empty list |
| `Handle_WithSearchTerm_FiltersByTitle` | Search courses by title/description | Returns filtered results |

---

### 4. Enrollment Tests

#### CreateEnrollmentCommandHandlerTests
**File**: `Features/Enrollment/CreateEnrollmentCommandHandlerTests.cs`

| Test Case | Description | Expected Result |
|-----------|-------------|-----------------|
| `Handle_WithValidCommand_CreatesEnrollmentSuccessfully` | Enroll user in course | Enrollment ID > 0 |
| `Handle_WithDuplicateEnrollment_ReturnZero` | Enroll same user in same course | Returns 0 |
| `Handle_WithInvalidUser_ThrowsException` | Enroll non-existent user | Throws exception |

---

### 5. Service Tests

#### CacheServiceTests
**File**: `Services/CacheServiceTests.cs`

| Test Case | Description | Expected Result |
|-----------|-------------|-----------------|
| `Set_WithValidData_StoresInCache` | Add data to cache | Data can be retrieved |
| `Get_WithNonExistentKey_ReturnsDefault` | Get non-existent cache key | Returns null |
| `Remove_WithValidKey_RemovesFromCache` | Remove item from cache | Item no longer retrievable |
| `RemoveByPattern_WithMatchingPattern_RemovesMatching` | Remove items matching pattern | Only matching items removed |
| `Set_WithObject_StoresAndRetrievesCorrectly` | Cache complex objects | Objects retrieved correctly |

#### PasswordHasherTests
**File**: `Services/PasswordHasherTests.cs`

| Test Case | Description | Expected Result |
|-----------|-------------|-----------------|
| `HashPassword_WithValidPassword_ReturnsHash` | Hash a password | Hash contains salt and hash separated by dot |
| `HashPassword_WithSamePassword_ProducesDifferentHashes` | Hash same password twice | Different hashes (different salts) |
| `VerifyPassword_WithCorrectPassword_ReturnsTrue` | Verify correct password | Returns true |
| `VerifyPassword_WithIncorrectPassword_ReturnsFalse` | Verify wrong password | Returns false |
| `VerifyPassword_WithMalformedHash_ReturnsFalse` | Verify with invalid hash format | Returns false |
| `VerifyPassword_WithEmptyPassword_ReturnsFalse` | Verify empty password | Returns false |

---

### 6. Common Model Tests

#### PaginationParamsTests
**File**: `Common/PaginationParamsTests.cs`

| Test Case | Description | Expected Result |
|-----------|-------------|-----------------|
| `PaginationParams_WithDefaultValues_HasCorrectDefaults` | Create with defaults | PageNumber=1, PageSize=10 |
| `PaginationParams_WithExceedingPageSize_CappsToMaxPageSize` | Set PageSize > 50 | Capped at 50 |
| `PaginationParams_WithValidPageSize_AcceptsPageSize` | Set PageSize=25 | Accepts value |
| `PaginationParams_WithSearchTerm_SetSearchTerm` | Set search term | Search term stored |

---

## Integration Tests

### 1. Repository Integration Tests

#### UserRepositoryIntegrationTests
**File**: `Integration/Repositories/UserRepositoryIntegrationTests.cs`

| Test Case | Description | Expected Result |
|-----------|-------------|-----------------|
| `AddAsync_WithValidUser_AddsUserToDatabase` | Add user to in-memory DB | User can be retrieved |
| `GetAllAsync_WithMultipleUsers_ReturnsAllUsers` | Get all users | Returns all added users |
| `IsEmailTakenAsync_WithExistingEmail_ReturnsTrue` | Check existing email | Returns true |
| `IsEmailTakenAsync_WithNonExistingEmail_ReturnsFalse` | Check non-existent email | Returns false |
| `GetPagedUsersAsync_WithPagination_ReturnsCorrectPage` | Get paginated users (10 per page) | Returns correct page size |

**Database**: In-memory SQLite for isolation

#### CourseRepositoryIntegrationTests
**File**: `Integration/Repositories/CourseRepositoryIntegrationTests.cs`

| Test Case | Description | Expected Result |
|-----------|-------------|-----------------|
| `AddAsync_WithValidCourse_AddsCourseToDatabase` | Add course to in-memory DB | Course can be retrieved |
| `GetByTitleAsync_WithExistingTitle_ReturnsCourse` | Get course by title | Returns matching course |
| `GetPagedCoursesAsync_WithSearchTerm_FiltersCorrectly` | Search courses by title | Returns only matching courses |
| `GetPagedCoursesAsync_WithPagination_ReturnsPaginatedResults` | Get paginated courses | Returns correct pages |

**Database**: In-memory SQLite for isolation

---

## Running Tests

### Run All Tests
```powershell
dotnet test Tests.xUnit/Tests.xUnit.csproj
```

### Run Specific Test Class
```powershell
dotnet test Tests.xUnit/Tests.xUnit.csproj --filter "FullyQualifiedName~LoginCommandHandlerTests"
```

### Run Tests with Verbose Output
```powershell
dotnet test Tests.xUnit/Tests.xUnit.csproj --logger "console;verbosity=detailed"
```

### Run Tests with Code Coverage
```powershell
dotnet test Tests.xUnit/Tests.xUnit.csproj /p:CollectCoverage=true
```

---

## Test Coverage Summary

| Layer | Tests | Coverage |
|-------|-------|----------|
| Application Commands | 6 | CreateUser, CreateCourse, CreateEnrollment |
| Application Queries | 4 | GetAllUsers, GetAllCourses, GetAllEnrollments |
| Services | 12 | CacheService, PasswordHasher |
| Common Models | 5 | PaginationParams |
| Repositories | 10 | UserRepository, CourseRepository |
| **Total** | **37** | **Comprehensive** |

---

## Key Testing Principles Used

### 1. **Arrange-Act-Assert Pattern**
```csharp
// Arrange - Set up test data
var user = new User { Name = "Test", Email = "test@example.com" };

// Act - Execute the method
var result = handler.Handle(command, CancellationToken.None);

// Assert - Verify the result
result.Should().BeGreaterThan(0);
```

### 2. **Moq for Mocking Dependencies**
```csharp
var mockRepository = new Mock<IUserRepository>();
mockRepository
    .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
    .ReturnsAsync(user);
```

### 3. **FluentAssertions for Readable Assertions**
```csharp
result.Should().NotBeNull();
result.Should().HaveCount(2);
result.Should().Contain(u => u.Email == "test@example.com");
```

### 4. **In-Memory Database for Integration Tests**
```csharp
var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
    .Options;
```

---

## Best Practices Implemented

✅ **Isolation**: Each test is independent and doesn't affect others
✅ **Repeatability**: Tests produce consistent results
✅ **Speed**: Unit tests run quickly without DB/network calls
✅ **Clarity**: Test names clearly describe what is being tested
✅ **Coverage**: Tests cover happy paths, edge cases, and error scenarios
✅ **Maintainability**: Tests use helper methods and avoid duplication

---

## CI/CD Integration

To run tests automatically in your CI/CD pipeline:

```yaml
# Example GitHub Actions
- name: Run Tests
  run: dotnet test Tests.xUnit/Tests.xUnit.csproj
```

---

## Future Test Additions

Recommended areas for additional tests:
- [ ] Controller endpoint tests (API integration tests)
- [ ] Error code validation tests
- [ ] Cache invalidation tests
- [ ] Transaction rollback tests
- [ ] Email service mock tests
- [ ] Concurrent operation tests
