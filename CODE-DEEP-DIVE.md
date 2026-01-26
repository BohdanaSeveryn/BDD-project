# Detailed Code Deep Dive - Booking System Tests
## A Comprehensive Learning Guide for C# Testing

This document provides an in-depth, line-by-line explanation of the Booking System test code. It's written as a learning resource for C# students, explaining not just WHAT the code does, but WHY it's written that way.

---

## Table of Contents

1. [C# Fundamentals Review](#c-fundamentals-review)
2. [Testing Framework Concepts](#testing-framework-concepts)
3. [File-by-File Deep Dive](#file-by-file-deep-dive)
4. [Common Patterns Explained](#common-patterns-explained)
5. [Best Practices and Reasoning](#best-practices-and-reasoning)

---

# C# Fundamentals Review

Before diving into the tests, let's review some C# concepts that appear frequently in this codebase.

## 1. Async/Await Pattern

### What is `async` and `await`?

```csharp
public async Task<LoginResult> LoginAsync(string apartmentNumber, string password)
{
    // This method is asynchronous
    await Task.Delay(1000); // Simulate waiting for database
    return new LoginResult { Success = true };
}
```

**Explanation:**
- `async` keyword tells C# that this method can use `await` and may take time to complete
- `await` pauses execution until an asynchronous operation completes, WITHOUT blocking the thread
- `Task<T>` is a container for an operation that will eventually return a value of type `T`

**Why use async?**
- **Non-blocking**: The thread isn't frozen while waiting. Other operations can run.
- **Performance**: Your application can handle more concurrent operations with fewer threads.
- **Responsive UI**: UI remains responsive while waiting for network/database operations.

### How it works:
```csharp
// Synchronous (BLOCKS - BAD for high-concurrency scenarios)
public LoginResult LoginSync(string apartment, string password)
{
    Thread.Sleep(1000); // Entire thread pauses - nothing else can happen
    return GetLoginResult();
}

// Asynchronous (NON-BLOCKING - GOOD)
public async Task<LoginResult> LoginAsync(string apartment, string password)
{
    await Task.Delay(1000); // Thread is freed - other operations can run
    return GetLoginResult();
}
```

---

## 2. Generics

### What are Generics?

```csharp
public interface IEmailService
{
    Task<bool> SendActivationEmailAsync(string email, string link);
    //   ^^^^
    //   Generic type parameter - specifies what this Task will return
}
```

**Explanation:**
- `<T>` is a **placeholder** for any type
- `Task<bool>` means "a Task that will eventually return a boolean value"
- `List<string>` means "a List that stores strings"

**Why use generics?**
- **Type Safety**: Compiler catches mistakes at compile-time, not runtime
- **Code Reuse**: One interface works with any return type
- **Clarity**: Reader immediately knows what type is returned

### Example:
```csharp
// WITHOUT generics (NOT TYPE SAFE - could fail at runtime)
public object GetValue() { return 42; }
int result = (int)GetValue(); // Need to cast - risky!

// WITH generics (TYPE SAFE - compiler checks)
public T GetValue<T>() where T : new() { return new T(); }
int result = GetValue<int>(); // Compiler ensures T is int
```

---

## 3. Interfaces

### What is an Interface?

```csharp
public interface IResidentService
{
    Task<bool> ActivateAccountAsync(Guid residentId, string password);
    Task<LoginResult> LoginAsync(string apartmentNumber, string password);
}
```

**Explanation:**
- Interface is a **contract** - defines what methods must exist
- It does NOT contain implementation (the actual code)
- Any class implementing this interface MUST have these methods
- The `I` prefix is a C# naming convention for interfaces

**Why use interfaces?**
- **Abstraction**: You don't care HOW it works, just that it works
- **Testing**: You can replace real implementations with mocks for testing
- **Loose Coupling**: Code depends on interface, not specific implementation
- **Flexibility**: Easy to swap implementations without changing code

### Example:
```csharp
// Interface (contract)
public interface IPaymentService
{
    Task<bool> ProcessPaymentAsync(decimal amount);
}

// Real implementation (actual code)
public class PaymentService : IPaymentService
{
    public async Task<bool> ProcessPaymentAsync(decimal amount)
    {
        // Actually charges credit card
        return await ChargeCard(amount);
    }
}

// Mock implementation (for testing)
public class MockPaymentService : IPaymentService
{
    public async Task<bool> ProcessPaymentAsync(decimal amount)
    {
        // Pretends to charge, but doesn't actually
        return true;
    }
}
```

---

## 4. Guid (Globally Unique Identifier)

### What is a Guid?

```csharp
var residentId = Guid.NewGuid();
// Produces: "7a8c6d2f-4e9b-4f2a-8c1d-9e2f7a8c6d2f"
```

**Explanation:**
- Guid is a unique identifier: almost impossible to generate the same one twice
- Uses 128 bits (32 hex characters)
- Each call to `Guid.NewGuid()` creates a new unique value

**Why use Guid instead of integers?**
- **Uniqueness**: Can generate IDs distributed across systems
- **Security**: Harder to guess sequential IDs
- **No Server Coordination**: Each system can generate IDs independently
- **Collision Prevention**: Virtually zero chance of duplicates

---

## 5. Access Modifiers (Public, Private, Internal)

### Why Different Access Levels?

```csharp
public class ResidentService        // Anyone can use this
{
    private string _apiKey;         // Only THIS class can access
    public async Task LoginAsync()   // Anyone can call this
    {
        RefreshToken();             // Only this class can call
    }
    
    private void RefreshToken()     // Only this class can use
    {
        _apiKey = GetNewToken();
    }
}
```

**Explanation:**
- `public`: Anyone can access (exposes your API)
- `private`: Only this class can access (hides implementation details)
- `internal`: Only this assembly (DLL/project) can access

**Why encapsulation matters:**
- **Hide Complexity**: Users don't need to know implementation
- **Prevent Misuse**: Can't accidentally call internal methods
- **Future Changes**: Can change private implementation without breaking users
- **Maintainability**: Clearer what's intended to be public vs internal

### Real-world analogy:
```csharp
// Like a car:
// - Steering wheel: public (user interacts)
// - Engine: private (user doesn't interact directly)
// - Internal wiring: private (user can't access)

// If everything was public, user might try to adjust engine timing manually!
```

---

# Testing Framework Concepts

Now that we've reviewed C# basics, let's understand the testing frameworks.

## 1. xUnit Testing Framework

### What is xUnit?

xUnit is a testing framework that makes it easy to write and run automated tests.

```csharp
namespace BookingSystem.Tests.Features;

public class AuthenticationTests  // Test class - contains multiple tests
{
    [Fact]  // Attribute indicating this is a test
    public async Task LoginFails_WhenAccountNotActivated()
    {
        // Test code here
    }
}
```

**Key Concept - [Fact] Attribute:**
- `[Fact]` tells xUnit "this method is a test - run it automatically"
- The method must be `public`
- The method can be synchronous or `async Task`
- xUnit discovers all [Fact] methods and executes them

### Why public methods?

```csharp
// NOT public - xUnit CAN'T find and run it
[Fact]
private async Task TestMethod() { }

// Public - xUnit CAN find and run it
[Fact]
public async Task TestMethod() { }
```

**Reasoning:**
- xUnit uses **reflection** (examining type information at runtime) to discover tests
- Reflection looks for `public` methods with `[Fact]` attribute
- If the method is `private`, reflection can't access it
- Therefore, test methods MUST be `public`

### Naming Convention

```csharp
[DisplayName("US-1: Successful account activation")]
public async Task SuccessfulAccountActivation_WhenValidActivationLinkAndPassword_AccountBecomesActive()
{
    // Name structure: [UnitOfWork]_[Scenario]_[ExpectedBehavior]
}
```

**Why this naming?**
- **Readability**: Name describes what the test does
- **Documentation**: Future developers understand test purpose
- **Search**: Easy to find tests for specific functionality
- **Debugging**: Failed test name immediately tells you what failed

---

## 2. Moq Mocking Framework

### What is Mocking?

Mocking is creating **fake** versions of real services for testing.

```csharp
// Real service (would hit actual database)
var emailService = new RealEmailService();

// Mock (fake version - we control what it does)
var emailServiceMock = new Mock<IEmailService>();
```

**Why mock?**
- **Isolation**: Test only the code you care about
- **Speed**: Don't hit real database/email service
- **Control**: You decide what the service returns
- **Repeatability**: Same result every run

### Setting Up Mocks - The `.Setup()` Method

```csharp
private readonly Mock<IEmailService> _emailServiceMock;

public AuthenticationTests()
{
    _emailServiceMock = new Mock<IEmailService>();
    // At this point, mock exists but has no configured behavior
}

// In test method:
[Fact]
public async Task ActivationEmailSent_WhenAccountCreated()
{
    // Configure mock: when SendActivationEmailAsync is called, return true
    _emailServiceMock
        .Setup(x => x.SendActivationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
        .ReturnsAsync(true);
    
    // Now when code calls: await emailService.SendActivationEmailAsync("test@email.com", "link")
    // The mock automatically returns true
}
```

**Breaking down `.Setup()`:**

```csharp
_emailServiceMock.Setup(x => x.SendActivationEmailAsync(...))
                 ^^^^^^
                 First parameter: lambda expression that selects the method to configure

x => x.SendActivationEmailAsync(It.IsAny<string>(), It.IsAny<string>())
^^    ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
parameter  this is a lambda: "for parameter x (the mock), select the method..."

It.IsAny<string>()
^^^^^^^^^^^^^^^^^^^^^^
"Match ANY string argument" - don't care what string is passed
```

### Different Return Types

```csharp
// Return a value
_serviceMock
    .Setup(x => x.GetUserAsync(It.IsAny<Guid>()))
    .ReturnsAsync(new User { Id = Guid.NewGuid() });

// Throw an exception
_serviceMock
    .Setup(x => x.ProcessAsync())
    .ThrowsAsync(new InvalidOperationException("Error!"));

// Execute custom code then return
_serviceMock
    .Setup(x => x.DeleteAsync(It.IsAny<Guid>()))
    .Callback(() => Console.WriteLine("Deleting..."))
    .ReturnsAsync(true);
```

**Why `.ReturnsAsync()`?**
```csharp
// Methods that return Task need ReturnsAsync
.Setup(x => x.AsyncMethod()).ReturnsAsync(value)

// Methods that return synchronous values need Returns
.Setup(x => x.SyncMethod()).Returns(value)

// Async methods MUST use ReturnsAsync, not Returns
// Using wrong one will cause runtime errors!
```

### Verifying Mock Calls - The `.Verify()` Method

```csharp
// Test that a method WAS called
_emailServiceMock.Verify(
    x => x.SendActivationEmailAsync("test@email.com", It.IsAny<string>()),
    Times.Once  // Should be called exactly once
);

// Test that a method was NOT called
_emailServiceMock.Verify(
    x => x.SendAccountDeletionEmailAsync(It.IsAny<string>()),
    Times.Never  // Should never be called
);

// Test that a method was called multiple times
_notificationServiceMock.Verify(
    x => x.NotifyAsync(It.IsAny<string>()),
    Times.Exactly(3)  // Called exactly 3 times
);
```

**When to use `.Verify()`:**
```csharp
[Fact]
public async Task ResidentNotified_WhenBookingCancelled()
{
    // Arrange
    var cancellationRequest = new AdminCancellationRequest { /*...*/ };
    
    _adminServiceMock
        .Setup(x => x.CancelBookingAsync(cancellationRequest))
        .ReturnsAsync(true);
    
    _notificationServiceMock
        .Setup(x => x.SendCancellationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
        .ReturnsAsync(true);
    
    // Act
    var cancelled = await _adminServiceMock.Object.CancelBookingAsync(cancellationRequest);
    var notified = await _notificationServiceMock.Object.SendCancellationEmailAsync("email", "reason");
    
    // Assert - verify the email was actually called
    _notificationServiceMock.Verify(
        x => x.SendCancellationEmailAsync(It.IsAny<string>(), It.IsAny<string>()),
        Times.Once);
    // This ensures that cancellation triggered the email
}
```

### `.Object` Property

```csharp
var mock = new Mock<IEmailService>();
// mock is the Mock wrapper

var realObject = mock.Object;
// realObject is the actual IEmailService to use in code
```

**Why this separation?**
```csharp
// The Mock object (wrapper)
// Has methods like .Setup(), .Verify(), .Reset()
_serviceMock.Setup(...);
_serviceMock.Verify(...);

// The Service object (the fake service itself)
// Has the methods from IEmailService interface
await _serviceMock.Object.SendEmailAsync("test@email.com");
```

---

## 3. FluentAssertions

### What is FluentAssertions?

FluentAssertions makes test assertions readable like English sentences.

```csharp
// Without FluentAssertions (hard to read)
Assert.NotNull(result);
Assert.Equal("john", result.Name);
Assert.True(result.IsActive);

// With FluentAssertions (reads like English)
result.Should().NotBeNull();
result.Name.Should().Be("john");
result.IsActive.Should().BeTrue();
```

**Why this matters:**
- Failed tests are easier to understand
- Non-developers can read test names and assertions
- Self-documenting code

### Common Assertions

```csharp
// Boolean checks
result.Should().BeTrue();
result.Should().BeFalse();

// Null checks
user.Should().NotBeNull();
email.Should().BeNullOrEmpty();

// Value comparisons
status.Should().Be("Active");
count.Should().Be(5);
price.Should().BeGreaterThan(10);
price.Should().BeLessThanOrEqualTo(100);

// String checks
email.Should().Contain("@");
name.Should().StartWith("John");
message.Should().EndWith("!");

// Collection checks
users.Should().HaveCount(3);           // Has exactly 3 items
users.Should().NotBeEmpty();           // Has at least 1 item
users.Should().Contain(expectedUser);  // Contains specific item
users.All(u => u.IsActive).Should().BeTrue(); // All satisfy condition

// Exception checks
Func<Task> act = async () => await service.DeleteAsync(Guid.Empty);
await act.Should().ThrowAsync<ArgumentException>();
```

### Why `Should()` Returns a Type That Has These Methods

```csharp
// Here's what happens internally:
public class AssertionHelper<T>
{
    private T _actual;
    
    public AssertionHelper<T>(T actual)
    {
        _actual = actual;
    }
    
    // Be method returns new assertion object for chaining
    public AssertionHelper<T> Be(T expected)
    {
        if (!_actual.Equals(expected))
            throw new AssertionFailedException("Not equal!");
        return this;  // Return self to allow chaining
    }
    
    public AssertionHelper<string> And => this;  // Fluent chaining
}

// Usage
var result = "Hello";
result.Should()      // Returns AssertionHelper<string>
    .Be("Hello")     // Compares, returns AssertionHelper for chaining
    .And            // Returns self for chaining
    .StartWith("H"); // Additional assertion
```

---

# File-by-File Deep Dive

Now let's examine each file in detail, explaining every important line.

---

## AuthenticationTests.cs - Complete Analysis

### File Location
`BookingSystem.Tests/Features/AuthenticationTests.cs`

### Overall Structure

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;                    // xUnit test framework
using Moq;                      // Mocking framework
using FluentAssertions;         // Fluent assertions

namespace BookingSystem.Tests.Features;  // Namespace - organizational structure
// "namespace BookingSystem.Tests.Features;" means this file is part of Features sub-namespace
// WHY? Keeps related tests grouped together, easier to navigate large projects
```

### Class 1: ResidentAccountActivationTests

```csharp
public class ResidentAccountActivationTests
{
    // WHY public? xUnit requires public classes to discover tests within them
    
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IResidentService> _residentServiceMock;
    
    // WHY private readonly?
    // - private: only this class accesses these mocks
    // - readonly: once set in constructor, can't be changed (prevents accidental modification)
    // - Mock<IEmailService>: stores the mock object for use in tests
    // WHY store as class fields? So ALL test methods can use them without re-creating
}
```

#### Constructor

```csharp
public ResidentAccountActivationTests()
// WHY public? xUnit creates new instance of test class for EACH test method
// This ensures tests don't interfere with each other
{
    _emailServiceMock = new Mock<IEmailService>();
    _residentServiceMock = new Mock<IResidentService>();
    // Create mock instances
    // These are EMPTY - no behavior configured yet
    // Each test will configure them as needed
}
```

**Why create fresh mocks for each test?**
```csharp
// BAD - mocks are shared across tests
public class BadTests
{
    private Mock<IService> _serviceMock = new Mock<IService>();
    
    [Fact]
    public void Test1()
    {
        _serviceMock.Setup(x => x.DoSomething()).Returns(true);
        // This affects Test2!
    }
    
    [Fact]
    public void Test2()
    {
        // _serviceMock.Setup() affects this test
        // Hard to debug - tests interfere with each other
    }
}

// GOOD - fresh mocks for each test
public class GoodTests
{
    private Mock<IService> _serviceMock;
    
    public GoodTests()
    {
        _serviceMock = new Mock<IService>();
        // Fresh instance created before EACH test
    }
}
```

#### Test Method 1: SuccessfulAccountActivation

```csharp
[Fact(DisplayName = "US-1: Successful account activation")]
// [Fact] tells xUnit this is a test
// DisplayName provides readable description shown in test reports
public async Task SuccessfulAccountActivation_WhenValidActivationLinkAndPassword_AccountBecomesActive()
{
    // WHY async Task? Because we're testing async methods
    // If we used synchronous Task, we'd need Task.Run() wrapper - less clean
    
    // ============ ARRANGE ============
    var residentId = Guid.NewGuid();
    // Create unique ID for test resident
    // WHY Guid? Simulates real scenario - IDs are unique identifiers
    
    var activationToken = "valid-token-123";
    var password = "SecurePassword123!";
    // WHY hardcoded strings for test? 
    // - These are test-specific values
    // - Don't change between runs
    // - Tests should be independent of external config
    
    var resident = new ResidentAccount
    {
        Id = residentId,
        Email = "resident@example.com",
        IsActive = false,  // Account not yet activated
        ActivationToken = activationToken,
        ActivationTokenExpiry = DateTime.UtcNow.AddHours(24)  // Token valid for 24 hours
    };
    // Create test data matching what database would return
    
    _residentServiceMock
        .Setup(x => x.GetResidentByIdAsync(residentId))
        // Configure mock: when GetResidentByIdAsync(residentId) is called
        .ReturnsAsync(resident);
        // Return our test resident object
    // WHY mock this? We don't want to hit actual database
    // In production, this would query database - for testing, we control it
    
    _residentServiceMock
        .Setup(x => x.ActivateAccountAsync(residentId, password))
        // Configure mock: when ActivateAccountAsync is called with these params
        .ReturnsAsync(true);
        // Return true (success)
    
    // ============ ACT ============
    var result = await _residentServiceMock.Object.ActivateAccountAsync(residentId, password);
    // Call the mocked service method
    // .Object accesses the actual IResidentService mock implementation
    // await waits for the async operation to complete
    // result contains the boolean return value
    
    // ============ ASSERT ============
    result.Should().BeTrue();
    // Verify the result is true
    // FluentAssertions makes this readable
    
    _residentServiceMock.Verify(
        x => x.ActivateAccountAsync(residentId, password), 
        Times.Once
    );
    // Verify ActivateAccountAsync was called EXACTLY ONCE with these parameters
    // WHY verify? Ensure the right method was called
    // Times.Once ensures it wasn't called 0 times or multiple times
}
```

#### Test Method 2: ActivationEmailSent_WhenAccountCreated

```csharp
[Fact(DisplayName = "US-1: Activation email sent when account is created")]
public async Task ActivationEmailSent_WhenAccountCreated()
{
    // ============ ARRANGE ============
    var residentEmail = "newresident@example.com";
    var residentId = Guid.NewGuid();
    // Test data setup
    
    _emailServiceMock
        .Setup(x => x.SendActivationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
        // It.IsAny<string>() means "match ANY string value"
        // We don't care what specific strings are passed
        // Just that the method is called with SOME strings
        .ReturnsAsync(true);
    // When email service is called, return true (success)
    
    // ============ ACT ============
    var emailSent = await _emailServiceMock.Object.SendActivationEmailAsync(
        residentEmail, 
        $"https://example.com/activate?token=unique-token");
    // Call the mocked email service
    // Pass actual email and a activation link
    
    // ============ ASSERT ============
    emailSent.Should().BeTrue();
    // Verify email was "sent" (mock returned true)
    
    _emailServiceMock.Verify(
        x => x.SendActivationEmailAsync(residentEmail, It.IsAny<string>()), 
        Times.Once);
    // Verify SendActivationEmailAsync was called exactly once
    // First parameter must be residentEmail (specific value)
    // Second parameter can be any string (It.IsAny)
    // WHY this verification? Ensure email service was triggered for this specific resident
}
```

**Comparing `.Setup()` with `.Verify()`:**

```csharp
// .Setup() configures mock BEHAVIOR (what to do when called)
_emailServiceMock
    .Setup(x => x.SendEmailAsync(It.IsAny<string>()))
    .ReturnsAsync(true);

// .Verify() checks mock WAS CALLED (after test executes)
_emailServiceMock
    .Verify(
        x => x.SendEmailAsync("test@email.com"),
        Times.Once);

// Timeline:
// 1. Setup: "IF someone calls SendEmailAsync, return true"
// 2. Act: actual test code runs (might or might not call the method)
// 3. Verify: "Was SendEmailAsync actually called with these parameters?"
```

#### Test Method 3: ActivationLinkExpired_WhenTokenExpiryTimeReached

```csharp
[Fact(DisplayName = "US-1: Activation link has time limit")]
public async Task ActivationLinkExpired_WhenTokenExpiryTimeReached()
{
    // ============ ARRANGE ============
    var residentId = Guid.NewGuid();
    var expiredToken = "expired-token-456";
    var resident = new ResidentAccount
    {
        Id = residentId,
        Email = "resident@example.com",
        ActivationToken = expiredToken,
        ActivationTokenExpiry = DateTime.UtcNow.AddHours(-1)  // ALREADY EXPIRED (1 hour ago)
        // WHY AddHours(-1)? Test scenario where token expired
    };
    
    _residentServiceMock
        .Setup(x => x.IsActivationTokenValidAsync(expiredToken))
        .ReturnsAsync(false);  // Mock returns false - token is invalid
    // WHY false? Expired tokens should be invalid
    
    // ============ ACT ============
    var isValid = await _residentServiceMock.Object.IsActivationTokenValidAsync(expiredToken);
    // Check if token is valid
    
    // ============ ASSERT ============
    isValid.Should().BeFalse();
    // Verify token is considered invalid (as expected for expired token)
}
```

---

### Class 2: ResidentLoginTests

```csharp
public class ResidentLoginTests
{
    private readonly Mock<IResidentService> _residentServiceMock;
    // Only need resident service mock for login tests
    
    public ResidentLoginTests()
    {
        _residentServiceMock = new Mock<IResidentService>();
    }
}
```

#### Test Method 1: SuccessfulLogin_WhenValidCredentialsAndAccountActive

```csharp
[Fact(DisplayName = "US-2: Successful login of active resident")]
public async Task SuccessfulLogin_WhenValidCredentialsAndAccountActive()
{
    // ============ ARRANGE ============
    var apartmentNumber = "A-101";      // Resident identifier
    var password = "ValidPassword123!";  // Resident's password
    var residentId = Guid.NewGuid();     // System-generated unique ID
    
    _residentServiceMock
        .Setup(x => x.LoginAsync(apartmentNumber, password))
        // Configure: when LoginAsync is called with exact apartment/password
        .ReturnsAsync(new LoginResult 
        { 
            Success = true,           // Login succeeded
            ResidentId = residentId,  // Return resident's ID
            Token = "valid-jwt-token" // Return authentication token
        });
    // WHY LoginResult object? Encapsulates login outcome
    // Better than returning separate values (Success, Id, Token)
    
    // ============ ACT ============
    var result = await _residentServiceMock.Object.LoginAsync(apartmentNumber, password);
    
    // ============ ASSERT ============
    result.Success.Should().BeTrue();
    // Verify login was successful
    
    result.ResidentId.Should().Be(residentId);
    // Verify returned ID matches what we expect
    
    result.Token.Should().NotBeNullOrEmpty();
    // Verify token was issued
    // WHY check token exists? Proves authentication credentials were generated
}
```

**Understanding LoginResult object:**

```csharp
// WHY this structure?
public class LoginResult
{
    public bool Success { get; set; }        // Did login succeed?
    public Guid ResidentId { get; set; }     // If successful, which resident?
    public string Token { get; set; }        // Authentication token for future requests
    public string ErrorMessage { get; set; } // If failed, why?
}

// Better than this:
public async Task<(bool, Guid, string, string)> LoginAsync(...)
// Hard to remember which position is what!

// Or this:
public async Task<bool> LoginAsync(...) // Only returns true/false - no useful data
```

#### Test Method 2: LoginFails_WhenAccountInactive

```csharp
[Fact(DisplayName = "US-2: Login denied for inactive account")]
public async Task LoginFails_WhenAccountInactive()
{
    // ============ ARRANGE ============
    var apartmentNumber = "B-205";
    var password = "password123";
    
    _residentServiceMock
        .Setup(x => x.LoginAsync(apartmentNumber, password))
        .ReturnsAsync(new LoginResult 
        { 
            Success = false,  // Login failed
            ErrorMessage = "Account is not activated"  // Explain why
        });
    // WHY return object with Success=false instead of throwing exception?
    // - Exception should be for UNEXPECTED errors
    // - Inactive account is EXPECTED scenario - should be handled gracefully
    
    // ============ ACT ============
    var result = await _residentServiceMock.Object.LoginAsync(apartmentNumber, password);
    
    // ============ ASSERT ============
    result.Success.Should().BeFalse();
    // Verify login was denied
    
    result.ErrorMessage.Should().Contain("not activated");
    // Verify error message explains why (contains text about activation)
    // WHY use .Contain() instead of exact match?
    // - More flexible - exact message might change
    // - Tests intent (account not activated) rather than exact text
}
```

**Expected Result vs Exception:**

```csharp
// Scenario 1: Invalid credentials (expected user error)
// Should return result with Success=false
var result = await loginService.LoginAsync("A-101", "wrongpassword");
result.Success.Should().BeFalse();

// Scenario 2: Database connection error (unexpected)
// Should throw exception
_databaseMock
    .Setup(x => x.GetUserAsync(It.IsAny<Guid>()))
    .ThrowsAsync(new DatabaseConnectionException("Cannot connect"));

// Why different handling?
// Expected errors = return result object (user can see error message)
// Unexpected errors = throw exception (indicates system problem)
```

---

### Class 3: AdminLoginTests

```csharp
public class AdminLoginTests
{
    private readonly Mock<IAdminService> _adminServiceMock;
    private readonly Mock<ITwoFactorService> _twoFactorServiceMock;
    
    public AdminLoginTests()
    {
        _adminServiceMock = new Mock<IAdminService>();
        _twoFactorServiceMock = new Mock<ITwoFactorService>();
        // Admin needs TWO mocks:
        // 1. Admin service for authentication
        // 2. Two-factor service for 2FA verification
        // Why? Tests are more realistic with multiple components
    }
}
```

#### Test: SuccessfulAdminLogin_WhenCredentialsAndTwoFactorCodeValid

```csharp
[Fact(DisplayName = "US-3: Successful admin login with 2FA")]
public async Task SuccessfulAdminLogin_WhenCredentialsAndTwoFactorCodeValid()
{
    // ============ ARRANGE ============
    var username = "admin@example.com";
    var password = "AdminPassword123!";
    var twoFactorCode = "123456";  // 6-digit 2FA code
    var adminId = Guid.NewGuid();  // Admin's unique ID
    
    // Configure admin service mock
    _adminServiceMock
        .Setup(x => x.AuthenticateAsync(username, password))
        // When admin credentials are verified
        .ReturnsAsync(true);  // Return true (credentials valid)
    // WHY separate from 2FA check?
    // 1. First verify credentials
    // 2. THEN ask for 2FA code
    // 3. THEN issue token
    // Authentication is multi-step
    
    // Configure 2FA mock
    _twoFactorServiceMock
        .Setup(x => x.ValidateTwoFactorCodeAsync(adminId, twoFactorCode))
        // When 2FA code is verified
        .ReturnsAsync(true);  // Return true (code is valid)
    
    // ============ ACT ============
    var authResult = await _adminServiceMock.Object.AuthenticateAsync(username, password);
    // Step 1: Verify credentials
    
    var twoFactorResult = await _twoFactorServiceMock.Object.ValidateTwoFactorCodeAsync(
        adminId, 
        twoFactorCode);
    // Step 2: Verify 2FA code
    
    // ============ ASSERT ============
    authResult.Should().BeTrue();  // Credentials valid
    twoFactorResult.Should().BeTrue();  // 2FA code valid
    // Both must be true for login success
}
```

**Why two-step authentication?**

```csharp
// Something you know (password)
// Something you have (phone with 2FA code)
// Together = stronger security

// If only password:
// - Hacker gets password -> gets access
// Password compromises = security breached

// If password + 2FA:
// - Hacker gets password -> STILL needs 2FA code
// - Hacker needs to compromise BOTH password AND phone
// Much harder to compromise both
```

#### Test: TwoFactorCodeSent_WhenAdminAuthenticated

```csharp
[Fact(DisplayName = "US-3: 2FA code sent via email")]
public async Task TwoFactorCodeSent_WhenAdminAuthenticated()
{
    // ============ ARRANGE ============
    var adminEmail = "admin@example.com";
    
    _twoFactorServiceMock
        .Setup(x => x.SendTwoFactorCodeAsync(adminEmail))
        // When asked to send 2FA code
        .ReturnsAsync(true);  // Return true (email sent successfully)
    
    // ============ ACT ============
    var result = await _twoFactorServiceMock.Object.SendTwoFactorCodeAsync(adminEmail);
    // Trigger sending of 2FA code
    
    // ============ ASSERT ============
    result.Should().BeTrue();
    // Verify code was sent (return value = true)
    
    _twoFactorServiceMock.Verify(
        x => x.SendTwoFactorCodeAsync(adminEmail), 
        Times.Once);
    // Verify SendTwoFactorCodeAsync was called exactly once
    // Ensures email service was triggered
    // If never called = user never gets the code!
}
```

#### Test: AdminAccessDenied_WhenTwoFactorCodeInvalid

```csharp
[Fact(DisplayName = "US-3: Access denied with invalid 2FA code")]
public async Task AdminAccessDenied_WhenTwoFactorCodeInvalid()
{
    // ============ ARRANGE ============
    var adminId = Guid.NewGuid();
    var invalidCode = "000000";  // Fake 2FA code
    
    _twoFactorServiceMock
        .Setup(x => x.ValidateTwoFactorCodeAsync(adminId, invalidCode))
        // When validating invalid code
        .ReturnsAsync(false);  // Return false (validation failed)
    // WHY false instead of exception?
    // Invalid 2FA code is EXPECTED scenario
    // Should be handled gracefully with login denial
    
    // ============ ACT ============
    var result = await _twoFactorServiceMock.Object.ValidateTwoFactorCodeAsync(
        adminId, 
        invalidCode);
    // Attempt to validate wrong code
    
    // ============ ASSERT ============
    result.Should().BeFalse();
    // Verify validation failed (as expected)
    // Login should be denied when result is false
}
```

---

## AdminAccountManagementTests.cs - Complete Analysis

### File Overview

This test file verifies that administrators can:
- Create resident accounts
- Delete resident accounts
- Update resident information

### Class: AdminAccountManagementTests

```csharp
public class AdminAccountManagementTests
{
    private readonly Mock<IAdminService> _adminServiceMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IResidentService> _residentServiceMock;
    // THREE mocks needed because account creation involves:
    // 1. Admin service to create account
    // 2. Email service to send activation email
    // 3. Resident service to manage residents
    
    public AdminAccountManagementTests()
    {
        _adminServiceMock = new Mock<IAdminService>();
        _emailServiceMock = new Mock<IEmailService>();
        _residentServiceMock = new Mock<IResidentService>();
    }
}
```

### Region: US-4 - Admin Creates Resident Account

#### Test 1: SuccessfulResidentAccountCreation_WhenValidDataProvided

```csharp
[Fact(DisplayName = "US-4: Successful resident account creation")]
public async Task SuccessfulResidentAccountCreation_WhenValidDataProvided()
{
    // ============ ARRANGE ============
    var adminId = Guid.NewGuid();  // Admin performing the action
    
    var residentData = new CreateResidentRequest
    {
        Name = "Ivan Petrenko",
        ApartmentNumber = "A-101",
        Email = "ivan.petrenko@example.com",
        Phone = "+380501234567"
    };
    // WHY CreateResidentRequest class?
    // - Encapsulates all required fields
    // - Easy to pass to methods
    // - Single object instead of multiple parameters
    // - Can add validation to the class
    
    _adminServiceMock
        .Setup(x => x.IsAdminAuthorizedAsync(adminId))
        // Check: is this user actually an admin?
        .ReturnsAsync(true);
    // WHY check authorization first?
    // Security: prevent non-admins from creating accounts
    
    _residentServiceMock
        .Setup(x => x.CreateResidentAsync(residentData))
        // Create the account
        .ReturnsAsync(new ResidentAccount 
        { 
            Id = Guid.NewGuid(), 
            IsActive = false  // New accounts start inactive
            // WHY inactive? Account not yet confirmed/activated
        });
    
    _emailServiceMock
        .Setup(x => x.SendActivationEmailAsync(residentData.Email, It.IsAny<string>()))
        // Send activation email to new resident
        .ReturnsAsync(true);
    // WHY send email? Resident needs to activate account
    
    // ============ ACT ============
    var resident = await _residentServiceMock.Object.CreateResidentAsync(residentData);
    // Create account
    
    var emailSent = await _emailServiceMock.Object.SendActivationEmailAsync(
        residentData.Email, 
        "https://example.com/activate");
    // Send activation link
    
    // ============ ASSERT ============
    resident.Should().NotBeNull();  // Account was created
    resident.IsActive.Should().BeFalse();  // Not yet activated
    emailSent.Should().BeTrue();  // Email was sent
    // All three verifications: account created, not active, email sent
}
```

**Understanding Request/Response Pattern:**

```csharp
// Instead of:
public async Task<Resident> CreateResidentAsync(
    string name, 
    string apartmentNumber, 
    string email, 
    string phone)
// Hard to remember parameter order!

// Use request object:
public async Task<ResidentAccount> CreateResidentAsync(CreateResidentRequest request)
// Clear what data is needed
// Easy to add more fields without changing method signature

// Request class:
public class CreateResidentRequest
{
    public string Name { get; set; }
    public string ApartmentNumber { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}
// Self-documenting
// Properties have names
```

#### Test 2: ResidentCreationFails_WhenEmailAlreadyExists

```csharp
[Fact(DisplayName = "US-4: Error when duplicating email")]
public async Task ResidentCreationFails_WhenEmailAlreadyExists()
{
    // ============ ARRANGE ============
    var residentData = new CreateResidentRequest
    {
        Name = "Maria Sidorenko",
        ApartmentNumber = "B-202",
        Email = "existing@example.com",  // This email already exists
        Phone = "+380509876543"
    };
    
    _residentServiceMock
        .Setup(x => x.CreateResidentAsync(residentData))
        .ThrowsAsync(new InvalidOperationException("Email already in use"));
    // WHY throw exception?
    // - This is an UNEXPECTED scenario for the service
    // - Violates business rule (unique emails)
    // - Should stop execution immediately
    
    // ============ ACT & ASSERT ============
    await Assert.ThrowsAsync<InvalidOperationException>(
        () => _residentServiceMock.Object.CreateResidentAsync(residentData));
    // Verify that InvalidOperationException was thrown
    // WHY Assert.ThrowsAsync?
    // - Specific exception type check
    // - Tests error handling
    // - Ensures code stops execution on error
}
```

**When to throw Exception vs Return Failure:**

```csharp
// Business rule violation = THROW EXCEPTION
public async Task CreateResidentAsync(CreateResidentRequest request)
{
    if (await EmailExistsAsync(request.Email))
        throw new InvalidOperationException("Email already in use");
    // This shouldn't happen if validation is correct
    // Stop execution immediately
}

// Expected user error = RETURN FAILURE RESULT
public async Task<LoginResult> LoginAsync(string apartment, string password)
{
    var resident = await GetResidentAsync(apartment);
    if (resident == null || !VerifyPassword(password))
        return new LoginResult { Success = false, ErrorMessage = "Invalid credentials" };
    // User entered wrong password = expected
    // Return result for UI to display error message
    // Don't throw exception
}

// Rule:
// - Exceptions = bug/unexpected condition
// - Result objects = expected failure scenario
```

---

## TestDataBuilders.cs - Complete Analysis

### Purpose and Structure

```csharp
namespace BookingSystem.Tests.Fixtures;

using System;
using System.Collections.Generic;

// WHY separate file for test data builders?
// - Keeps test code clean
// - Reusable across multiple test classes
// - Easy to maintain test data
// - One place to update test values
```

### Class: ResidentTestDataBuilder

```csharp
public static class ResidentTestDataBuilder
// WHY static class?
// - Doesn't need instance state
// - Methods are static (called as ResidentTestDataBuilder.BuildValidResident())
// - Simpler than creating object
{
    public static ResidentAccount BuildValidResident()
    // WHY this method name?
    // - "Build" suggests building object from parts
    // - "Valid" means object has all required fields with valid values
    // - "Resident" specifies what type of object
    // Name tells you what it does
    {
        return new ResidentAccount
        {
            Id = Guid.NewGuid(),  // Generate unique ID
            Name = "Test Resident",
            Email = "test@example.com",
            Phone = "+380501234567",
            ApartmentNumber = "A-101",
            IsActive = false,  // New residents start inactive
            ActivationToken = Guid.NewGuid().ToString(),
            // WHY convert Guid to string?
            // Token stored as string in database
            // Simulates real-world scenario
            ActivationTokenExpiry = DateTime.UtcNow.AddHours(24)
            // Token valid for 24 hours from now
            // WHY .UtcNow instead of .Now?
            // UTC is standard for databases (timezone-independent)
        };
    }
    
    public static CreateResidentRequest BuildValidCreateRequest()
    // Request object for creating resident
    // Contains data that would come from UI form
    {
        return new CreateResidentRequest
        {
            Name = "New Resident",
            ApartmentNumber = "B-202",
            Email = "new.resident@example.com",
            Phone = "+380509876543"
        };
    }
    
    public static UpdateResidentRequest BuildValidUpdateRequest()
    // Request object for updating resident
    // Different from CreateResidentRequest
    // WHY separate classes?
    // - Different requirements
    // - Create needs: Name, Email, Phone, Apartment
    // - Update might need: Name, Email, Phone (not Apartment)
    {
        return new UpdateResidentRequest
        {
            Name = "Updated name",
            Email = "updated@example.com",
            Phone = "+380505555555",
            ApartmentNumber = "C-303"
        };
    }
}
```

**Builder Pattern Benefits:**

```csharp
// Without builder (hard to create test data):
var resident = new ResidentAccount();
resident.Id = Guid.NewGuid();
resident.Name = "Test";
resident.Email = "test@email.com";
resident.Phone = "+380501234567";
// Many lines, easy to forget fields

// With builder (one method call):
var resident = ResidentTestDataBuilder.BuildValidResident();
// Clear intention
// All fields set to valid values
// Easy to understand what "valid resident" means
```

### Class: BookingTestDataBuilder

```csharp
public static class BookingTestDataBuilder
{
    public static List<TimeSlot> BuildDailyTimeSlots()
    // WHY return List<TimeSlot>?
    // - Tests often need multiple slots
    // - Simulates full day schedule
    // - Can test filtering, searching on multiple slots
    {
        return new List<TimeSlot>
        {
            // 10 time slots covering 8 AM to 6 PM
            // Alternating available/unavailable
            new() { 
                Id = Guid.NewGuid(), 
                StartTime = new TimeOnly(8, 0),   // 8:00 AM
                EndTime = new TimeOnly(9, 0),     // 9:00 AM
                IsAvailable = true 
            },
            new() { 
                Id = Guid.NewGuid(), 
                StartTime = new TimeOnly(9, 0), 
                EndTime = new TimeOnly(10, 0), 
                IsAvailable = true 
            },
            // WHY TimeOnly instead of DateTime?
            // - TimeOnly represents just the time (8:00 AM)
            // - No date component needed
            // - Cleaner than DateTime when you only care about time
            
            new() { 
                Id = Guid.NewGuid(), 
                StartTime = new TimeOnly(10, 0), 
                EndTime = new TimeOnly(11, 0), 
                IsAvailable = false  // Already booked
            },
            // Mixed available/unavailable
            // Tests can verify calendar shows both
            
            // ... more slots ...
        };
    }
    
    public static Booking BuildValidBooking(Guid residentId, Guid timeSlotId)
    // WHY parameters?
    // These vary between tests
    // Different tests need different resident/slot IDs
    {
        return new Booking
        {
            Id = Guid.NewGuid(),
            ResidentId = residentId,    // Use passed parameter
            TimeSlotId = timeSlotId,    // Use passed parameter
            Facility = "Laundry room",
            Date = DateTime.Today.AddDays(1),  // Tomorrow
            Time = "10:00 - 11:00",
            Status = "Confirmed",
            CreatedAt = DateTime.UtcNow
        };
    }
}
```

**DateTime.Today vs DateTime.UtcNow:**

```csharp
// DateTime.Today
DateTime.Today  // Returns 2026-01-26 00:00:00 (today at midnight)
// Useful for: booking dates, calendar dates (just the day)

// DateTime.UtcNow
DateTime.UtcNow  // Returns 2026-01-26 14:30:45.123456 (now in UTC)
// Useful for: audit timestamps, token expiration times (need exact moment)

// Booking date = just the day (use Today)
Date = DateTime.Today.AddDays(1)

// Token created timestamp = exact moment (use UtcNow)
CreatedAt = DateTime.UtcNow
```

### Class: TestConstants

```csharp
public static class TestConstants
// WHY static class of constants?
// - Single source of truth for test values
// - Easy to update all tests at once
// - Prevents typos (compile error if constant name wrong)
// - Clear what values are "correct" for tests
{
    public const string ValidResidentEmail = "resident@example.com";
    public const string ValidAdminEmail = "admin@example.com";
    public const string InvalidEmail = "invalid-email";
    // WHY different valid/invalid constants?
    // Tests need both working and broken scenarios
    
    public const string ValidPassword = "ValidPassword123!@#";
    // WHY complex password?
    // Real password validation typically requires:
    // - Uppercase letters (V)
    // - Lowercase letters (alid)
    // - Numbers (123)
    // - Special characters (!@#)
    // Test should verify real-world passwords work
    
    public const string WeakPassword = "weak";
    public const string InvalidPassword = "wrong";
    // These should FAIL validation
    
    // ... more constants ...
    
    public static readonly TimeOnly MorningSlotStart = new TimeOnly(8, 0);
    // WHY readonly instead of const?
    // const is for simple values (strings, numbers)
    // readonly is for complex objects (TimeOnly)
    
    public const string SlotAlreadyBookedError = "This slot just reserved";
    // Single source of truth for error message
    // If error message changes in code, update here
    // All tests automatically use new message
}
```

**Using Constants in Tests:**

```csharp
// WITHOUT constants (BAD - hard to maintain)
[Fact]
public void Test1() {
    var email = "resident@example.com";
    // ... test code
}

[Fact]
public void Test2() {
    var email = "resident@example.com";  // Duplication!
    // ... test code
}

[Fact]
public void Test3() {
    var email = "resident@example.com";  // Duplication again!
    // ... test code
}

// If email should change, must update 3 places!

// WITH constants (GOOD)
[Fact]
public void Test1() {
    var email = TestConstants.ValidResidentEmail;
    // ... test code
}

// If email should change, update TestConstants.ValidResidentEmail once
// All tests automatically use new value
```

### Class: BookingSystemFixture

```csharp
[CollectionDefinition("Booking System Collection")]
public class BookingSystemCollection : ICollectionFixture<BookingSystemFixture>
{
    // WHY this class?
    // Tells xUnit to use BookingSystemFixture for collection of tests
    // Only needed if tests MUST share state
    // (Most tests DON'T need this - independent is better)
}

public class BookingSystemFixture : IDisposable
// WHY implement IDisposable?
// For cleanup after tests
{
    public Guid TestResidentId { get; private set; }
    public Guid TestAdminId { get; private set; }
    public string TestFacilityId { get; private set; }
    public DateTime TestBookingDate { get; private set; }
    // Shared test data for all tests in collection
    
    public BookingSystemFixture()
    // Constructor called ONCE per test collection
    {
        TestResidentId = Guid.NewGuid();
        TestAdminId = Guid.NewGuid();
        TestFacilityId = Guid.NewGuid().ToString();
        TestBookingDate = DateTime.Today.AddDays(1);
        // Initialize shared test data
    }
    
    public void Dispose()
    // Called after all tests complete
    // Cleanup resources
    {
        // Cleanup code here if needed
        // (In this test, nothing to clean up)
    }
}
```

---

## TestModelsAndInterfaces.cs - Complete Analysis

### Purpose

Defines all interfaces that will be mocked and all data models used in tests.

### Interface: IResidentService

```csharp
public interface IResidentService
// WHY interface?
// - Tests mock this interface
// - Real code implements this interface
// - Same interface used everywhere = consistency
{
    Task<bool> ActivateAccountAsync(Guid residentId, string password);
    // Parameter: Guid instead of string
    // WHY? More type-safe than string apartment number
    // Impossible to pass invalid Guid
    
    Task<bool> IsActivationTokenValidAsync(string token);
    // Returns bool: is token valid?
    // Alternative: throw exception if invalid
    // But returning bool is cleaner for expected scenarios
    
    Task<LoginResult> LoginAsync(string apartmentNumber, string password);
    // Returns LoginResult object (not just bool)
    // WHY? Need to return: Success (bool) + Token (string) + ErrorMessage
    // Single object contains all info
    
    Task<ResidentAccount> GetResidentByIdAsync(Guid residentId);
    // Returns complete ResidentAccount object
    // Caller can access any property they need
    
    Task<ResidentAccount> CreateResidentAsync(CreateResidentRequest request);
    // Parameter: Request object
    // WHY not individual parameters?
    // - Cleaner method signature
    // - Easy to extend (add more fields to request)
    // - Clear what data is needed
}
```

### Data Model: ResidentAccount

```csharp
public class ResidentAccount
{
    public Guid Id { get; set; }
    // WHY public getter/setter for everything?
    // - Models are simple data containers
    // - No complex logic
    // - Easy to construct in tests
    
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string ApartmentNumber { get; set; }
    
    public bool IsActive { get; set; }
    // Is this account activated?
    // true = can log in
    // false = must complete activation first
    
    public string ActivationToken { get; set; }
    // Token sent to resident's email
    // Resident must click link with this token to activate
    
    public DateTime? ActivationTokenExpiry { get; set; }
    // ? means nullable
    // WHY nullable?
    // - New resident: has token (not null)
    // - Activated resident: no token (null)
    // - Deleted resident: no token (null)
}
```

**Nullable Types (`?`):**

```csharp
// Without nullable
public DateTime TokenExpiry { get; set; }  // Must have a value
// Problem: What if resident is already activated?
// Can't represent "no expiry date"

// With nullable
public DateTime? TokenExpiry { get; set; }  // Can be null
// null means "no token/no expiry date"
// Activated residents have null

// Usage:
if (resident.TokenExpiry == null)
    Console.WriteLine("Account is already activated");
else if (resident.TokenExpiry < DateTime.UtcNow)
    Console.WriteLine("Token expired");
else
    Console.WriteLine("Token still valid");
```

### Interface: IBookingService

```csharp
public interface IBookingService
{
    Task<List<TimeSlot>> GetAvailabilityAsync(Guid facilityId, DateTime bookingDate);
    // Returns List<TimeSlot>
    // WHY List instead of IEnumerable?
    // - List is concrete (definite collection)
    // - Easy to test: can count items, index by position
    // - IEnumerable is more abstract (harder for tests)
    
    Task<bool> SelectTimeSlotAsync(Guid timeSlotId);
    // Select a slot (in UI shopping cart style)
    // Not confirmed yet, just selected
    
    Task<bool> ClearSelectionAsync();
    // Unselect current slot
    // Back to empty selection
    
    Task<Booking> ConfirmBookingAsync(CreateBookingRequest request);
    // Actually create the booking
    // Now it's permanent (in database)
    
    Task<List<TimeSlot>> RefreshAvailabilityAsync(Guid facilityId, DateTime bookingDate);
    // Get latest availability
    // WHY separate from GetAvailabilityAsync?
    // - GetAvailabilityAsync: get availability for view
    // - RefreshAvailabilityAsync: force refresh (check again)
    // If another user just booked slot, refresh sees new status
    
    Task<List<Booking>> GetMyBookingsAsync(Guid residentId, DateTime bookingDate);
    // Get bookings for THIS resident on specific date
    // Only their bookings, not others'
    
    Task<List<Booking>> GetCalendarBookingsAsync(Guid facilityId, DateTime bookingDate);
    // Get ALL bookings for facility on date
    // Shows what's booked (all residents)
    
    Task<BookingDetails> GetBookingDetailsAsync(Guid bookingId);
    // Get details of single booking
    
    Task<bool> IsUserAuthorizedAsync(Guid residentId);
    // Is this user authenticated/authorized?
    // Access control check
    
    Task<List<Booking>> GetUpcomingBookingsAsync(Guid residentId);
    // Get all future bookings for resident
    // No date parameter = all upcoming dates
    
    Task<bool> CancelBookingAsync(Guid bookingId, Guid residentId);
    // Cancel booking
    // Two parameters: which booking to cancel + who's canceling
    // WHY both? Verify resident owns booking
    // Prevent canceling other residents' bookings
    
    Task<bool> CanCancelBookingAsync(Guid bookingId);
    // Can this booking be canceled?
    // May have time restrictions
    // (Can't cancel if less than 24 hours away)
    
    Task<bool> CanResidentCancelBookingAsync(Guid bookingId, Guid residentId);
    // Does this resident have permission to cancel?
    // Access control + business rules
}
```

**Parameter Design Pattern:**

```csharp
// Method design: when to pass multiple parameters?

// Too many parameters (hard to remember order)
public void CreateBooking(Guid residentId, Guid facilityId, DateTime date, 
    TimeOnly start, TimeOnly end, string notes) { }

// Better: use request object
public void CreateBooking(CreateBookingRequest request) { }

public class CreateBookingRequest
{
    public Guid ResidentId { get; set; }
    public Guid FacilityId { get; set; }
    public DateTime Date { get; set; }
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
    public string Notes { get; set; }
}

// But TWO parameters can be ok if logically related
public async Task<bool> CancelBookingAsync(Guid bookingId, Guid residentId)
// Makes sense: which booking to cancel (ID) + who's canceling (for permission check)
```

---

## BookingManagementTests.cs - Complete Analysis

### Class: BookingConfirmationTests

```csharp
public class BookingConfirmationTests
{
    private readonly Mock<IBookingService> _bookingServiceMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
}
```

#### Test: SuccessfulBookingConfirmation_WhenSlotSelected

```csharp
[Fact(DisplayName = "US-9: Successful booking confirmation")]
public async Task SuccessfulBookingConfirmation_WhenSlotSelected()
{
    // ============ ARRANGE ============
    var residentId = Guid.NewGuid();
    var timeSlotId = Guid.NewGuid();
    
    var bookingRequest = new CreateBookingRequest 
    { 
        ResidentId = residentId, 
        TimeSlotId = timeSlotId 
    };
    // Request object encapsulates booking intent
    
    var booking = new Booking 
    { 
        Id = Guid.NewGuid(), 
        ResidentId = residentId, 
        TimeSlotId = timeSlotId, 
        Status = "Confirmed",
        CreatedAt = DateTime.UtcNow
    };
    // What database returns after successful creation
    
    _bookingServiceMock
        .Setup(x => x.ConfirmBookingAsync(bookingRequest))
        .ReturnsAsync(booking);
    // Configure mock to return booking
    
    // ============ ACT ============
    var result = await _bookingServiceMock.Object.ConfirmBookingAsync(bookingRequest);
    // Attempt to confirm booking
    
    // ============ ASSERT ============
    result.Should().NotBeNull();
    // Booking was created (returned object)
    
    result.Status.Should().Be("Confirmed");
    // Status is confirmed
    
    result.ResidentId.Should().Be(residentId);
    // Correct resident owns booking
    // WHY verify this? Ensure resident can't book for others
}
```

#### Test: BookingFails_WhenSlotBookedDuringProcess

```csharp
[Fact(DisplayName = "US-10: Slot reserved by another resident during booking process")]
public async Task BookingFails_WhenSlotBookedDuringProcess()
{
    // ============ ARRANGE ============
    var residentId = Guid.NewGuid();
    var timeSlotId = Guid.NewGuid();
    var bookingRequest = new CreateBookingRequest 
    { 
        ResidentId = residentId, 
        TimeSlotId = timeSlotId 
    };
    
    _bookingServiceMock
        .Setup(x => x.ConfirmBookingAsync(bookingRequest))
        .ThrowsAsync(new InvalidOperationException(
            "This slot just reserved by another resident"));
    // Simulate: while this user was confirming, another user grabbed the slot
    // Race condition scenario
    
    // ============ ACT & ASSERT ============
    await Assert.ThrowsAsync<InvalidOperationException>(
        () => _bookingServiceMock.Object.ConfirmBookingAsync(bookingRequest));
    // Verify exception is thrown (and correct message)
    // WHY exception?
    // - Unexpected situation (race condition)
    // - System should stop immediately
    // - UI can display error to user
}
```

**Race Condition Example:**

```csharp
// Timeline of race condition:

// Time 1: User A clicks on slot 10:00-11:00 (selects in UI)
// Time 2: User B clicks on same slot (selects in UI)
// Time 3: User A confirms booking (posts to server)
//         Server creates booking for A
// Time 4: User B confirms booking (posts to server)
//         Server already has booking for A in that slot!
//         This call FAILS
//         A got the slot, B gets error

// Testing this:
_bookingServiceMock
    .Setup(x => x.ConfirmBookingAsync(request))
    .ThrowsAsync(new InvalidOperationException("Slot already booked"));
// Simulates the race condition

// WHY important to test?
// Race conditions are hard to reproduce
// Mock makes it easy to test
```

---

## NotificationTests.cs - Complete Analysis

### Class: NotificationTests

```csharp
public class NotificationTests
{
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<IBookingService> _bookingServiceMock;
    // Three mocks to test email notifications
}
```

#### Test: BookingConfirmationEmailSent_WhenBookingConfirmed

```csharp
[Fact(DisplayName = "US-22: Booking confirmation received")]
public async Task BookingConfirmationEmailSent_WhenBookingConfirmed()
{
    // ============ ARRANGE ============
    var residentEmail = "resident@example.com";
    var bookingConfirmation = new BookingConfirmationEmail
    {
        ResidentEmail = residentEmail,
        FacilityName = "Laundry room",
        Date = DateTime.Today.AddDays(1),
        Time = "10:00 - 11:00",
        ConfirmationToken = Guid.NewGuid().ToString()
    };
    // Email content object
    // WHY create class instead of passing individual strings?
    // - Email content is cohesive unit
    // - Easy to add more fields later
    // - Type-safe (can't pass string where Email expected)
    
    _emailServiceMock
        .Setup(x => x.SendBookingConfirmationAsync(bookingConfirmation))
        // Email service will send this confirmation
        .ReturnsAsync(true);
    // Returns true = email sent successfully
    
    // ============ ACT ============
    var sent = await _emailServiceMock.Object.SendBookingConfirmationAsync(
        bookingConfirmation);
    // Call email service
    
    // ============ ASSERT ============
    sent.Should().BeTrue();
    // Email was sent
    
    _emailServiceMock.Verify(
        x => x.SendBookingConfirmationAsync(It.Is<BookingConfirmationEmail>(
            e => e.ResidentEmail == residentEmail && 
                 e.FacilityName == "Laundry room")),
        Times.Once);
    // Verify email was called with correct content
    // It.Is<> creates custom matcher
    // Checks specific properties
}
```

**Using `It.Is<>` for Complex Verification:**

```csharp
// Simple verification (any object)
_emailServiceMock.Verify(
    x => x.SendEmail(It.IsAny<EmailMessage>()),
    Times.Once);

// Complex verification (specific properties)
_emailServiceMock.Verify(
    x => x.SendEmail(It.Is<EmailMessage>(
        e => e.To == "user@email.com" &&
             e.Subject.Contains("Booking") &&
             e.Body != null)),
    Times.Once);
// WHY It.Is?
// - Verify method was called with specific data
// - Not just any EmailMessage, but one with these properties
// - Ensures email contains correct information
```

---

# Common Patterns Explained

## Pattern 1: AAA (Arrange-Act-Assert)

Every test follows this structure:

```csharp
[Fact]
public async Task TestMethod()
{
    // ============ ARRANGE ============
    // Setup test data and mocks
    var userId = Guid.NewGuid();
    _userServiceMock.Setup(...).ReturnsAsync(...);
    
    // ============ ACT ============
    // Execute the code being tested
    var result = await _userServiceMock.Object.GetUserAsync(userId);
    
    // ============ ASSERT ============
    // Verify the result is correct
    result.Should().NotBeNull();
}
```

**Why AAA?**
- **Clarity**: Reader immediately understands test flow
- **Maintenance**: Easy to find what's being tested
- **Debugging**: Failed test = know which section failed (setup/execution/verification)

---

## Pattern 2: Mocking Dependencies

```csharp
public class ComplexServiceTests
{
    private readonly Mock<IEmailService> _emailMock;
    private readonly Mock<IDatabaseService> _dbMock;
    
    public ComplexServiceTests()
    {
        _emailMock = new Mock<IEmailService>();
        _dbMock = new Mock<IDatabaseService>();
    }
    
    [Fact]
    public async Task CreateUserAndSendEmail_Success()
    {
        // Setup what database should return
        _dbMock
            .Setup(x => x.CreateUserAsync(It.IsAny<User>()))
            .ReturnsAsync(new User { Id = Guid.NewGuid() });
        
        // Setup what email service should do
        _emailMock
            .Setup(x => x.SendWelcomeEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        
        // Now test: these mocks control behavior
        // Don't need real database or email service!
    }
}
```

**Why mock?**
- **Speed**: No database queries, email sending
- **Control**: Decide exactly what service returns
- **Repeatability**: Same result every run
- **Isolation**: Test one thing at a time

---

## Pattern 3: Testing Async Methods

```csharp
// Methods that return Task must be tested with async

// Method being tested
public async Task<User> GetUserAsync(Guid id)
{
    var user = await _database.GetUserAsync(id);
    return user;
}

// Test for this method
[Fact]
public async Task GetUserAsync_ReturnsUser()
{
    var userId = Guid.NewGuid();
    
    _databaseMock
        .Setup(x => x.GetUserAsync(userId))
        .ReturnsAsync(new User { Id = userId });
    // Must use ReturnsAsync for async methods!
    
    var result = await _databaseMock.Object.GetUserAsync(userId);
    // Must await the call
    
    result.Should().NotBeNull();
}

// WRONG - synchronous test won't work
[Fact]
public void GetUserAsync_ReturnsUser()  // NOT async
{
    // Can't use await here
    // Will deadlock or error
}
```

**Async Best Practices:**
```csharp
// ✓ CORRECT
[Fact]
public async Task MyAsyncTest()
{
    var result = await asyncMethod();
}

// ✗ WRONG
[Fact]
public void MyAsyncTest()
{
    asyncMethod().Wait();  // Deadlock risk!
}

// ✗ WRONG
[Fact]
public Task MyAsyncTest()  // Fire and forget
{
    return asyncMethod();  // Test completes before method finishes
}
```

---

# Best Practices and Reasoning

## 1. Test Naming Convention

```csharp
// Format: [UnitOfWork]_[ScenarioUnderTest]_[ExpectedBehavior]

// Examples:
LoginAsync_WhenValidCredentialsAndAccountActive_ReturnsSuccessAndToken()
// What it tests: LoginAsync method
// Scenario: valid credentials + active account
// Expected: returns success and token

CreateResidentAsync_WhenEmailAlreadyExists_ThrowsInvalidOperationException()
// What it tests: CreateResidentAsync
// Scenario: email already exists
// Expected: throws exception

GetBookingAvailability_WhenNoSlotsBetweenDates_ReturnsEmptyList()
```

**Benefits:**
- Self-documenting
- Future developers understand test purpose immediately
- Finding related tests is easy
- Failed tests in reports are immediately clear

---

## 2. Encapsulation with Access Modifiers

```csharp
public class BookingService
{
    // PUBLIC - others can use
    public async Task<Booking> ConfirmBookingAsync(CreateBookingRequest request)
    {
        // Validate request
        ValidateRequest(request);  // Private method
        
        // Check availability
        var isAvailable = await CheckAvailabilityAsync(request.SlotId);
        
        // Create booking
        return await _database.CreateAsync(booking);
    }
    
    // PRIVATE - only this class uses
    private void ValidateRequest(CreateBookingRequest request)
    {
        if (string.IsNullOrEmpty(request.ResidentId))
            throw new ArgumentException("Resident ID required");
    }
    
    // PRIVATE - internal helper
    private async Task<bool> CheckAvailabilityAsync(Guid slotId)
    {
        return await _database.IsSlotAvailableAsync(slotId);
    }
}

// Why?
// Public: ConfirmBookingAsync (what users call)
// Private: ValidateRequest, CheckAvailabilityAsync (internal details)
// Users don't need to know HOW it works, just that it works
```

---

## 3. Return Objects vs Exceptions

```csharp
// Expected error → Return Result Object
public async Task<LoginResult> LoginAsync(string apartment, string password)
{
    var user = await GetUserAsync(apartment);
    
    if (user == null)
        return new LoginResult 
        { 
            Success = false, 
            ErrorMessage = "User not found" 
        };
    // Expected scenario: user doesn't exist
    // UI can display friendly error message
    
    if (!VerifyPassword(password))
        return new LoginResult 
        { 
            Success = false, 
            ErrorMessage = "Invalid password" 
        };
    // Expected scenario: wrong password
    // UI can display "Credentials incorrect"
    
    return new LoginResult 
    { 
        Success = true, 
        Token = GenerateToken(user) 
    };
}

// Unexpected error → Throw Exception
public async Task ConfirmBookingAsync(CreateBookingRequest request)
{
    if (request == null)
        throw new ArgumentNullException(nameof(request));
    // Unexpected: null request = programmer error
    // Should never happen in normal operation
    
    var slot = await GetSlotAsync(request.SlotId);
    if (slot == null)
        throw new InvalidOperationException("Slot not found");
    // Unexpected: slot should always exist
    // Data integrity problem
    
    if (!slot.IsAvailable)
        throw new InvalidOperationException("Slot already booked");
    // WAIT! This could be expected (race condition)
    // Or throw? Depends on business logic
}
```

**Rule:**
- **Expected errors** (wrong password, not found, etc) → return Result object
- **Unexpected errors** (null params, data corruption) → throw Exception

---

## 4. Mock Verification vs Assertion

```csharp
[Fact]
public async Task BookingConfirmed_SendsEmail()
{
    // Setup
    _bookingServiceMock
        .Setup(x => x.ConfirmBookingAsync(It.IsAny<CreateBookingRequest>()))
        .ReturnsAsync(new Booking { Id = Guid.NewGuid() });
    
    _emailServiceMock
        .Setup(x => x.SendConfirmationAsync(It.IsAny<string>()))
        .ReturnsAsync(true);
    
    // Act
    var booking = await _bookingServiceMock.Object.ConfirmBookingAsync(request);
    var emailSent = await _emailServiceMock.Object.SendConfirmationAsync("user@email.com");
    
    // Assert - verify results
    booking.Should().NotBeNull();
    emailSent.Should().BeTrue();
    // These use FluentAssertions
    
    // Verify - verify methods were called
    _emailServiceMock.Verify(
        x => x.SendConfirmationAsync("user@email.com"),
        Times.Once);
    // Verify is from Moq
    // Checks that mock method was invoked
}
```

**Difference:**
- **Assert**: Verify returned values (what the method returned)
- **Verify**: Verify methods were called (what side effects happened)

---

## 5. Testing Edge Cases and Negative Scenarios

```csharp
// GOOD test class covers multiple scenarios

[Fact]
public async Task Login_SuccessWithValidCredentials()
{
    // Happy path - everything works
}

[Fact]
public async Task Login_FailsWithInvalidPassword()
{
    // Wrong password scenario
}

[Fact]
public async Task Login_FailsWithInactiveAccount()
{
    // Account not activated yet
}

[Fact]
public async Task Login_FailsWithDeletedAccount()
{
    // Account was deleted
}

[Fact]
public async Task Login_FailsWithNullCredentials()
{
    // Null parameters
}

// WHY test all these?
// - Ensures code handles all scenarios gracefully
// - One bug might be in successful path
// - Another in error path
// - Need comprehensive coverage
```

---

# Summary

This deep dive covered:

1. **C# Fundamentals**
   - async/await (non-blocking operations)
   - Generics (type-safe containers)
   - Interfaces (contracts for implementations)
   - Guid (unique identifiers)
   - Access modifiers (encapsulation)

2. **Testing Frameworks**
   - xUnit basics and [Fact] attribute
   - Moq mocking and .Setup()/.Verify()
   - FluentAssertions readable syntax

3. **File-by-File Analysis**
   - Every class and major test method explained
   - WHY each line is written that way
   - Pattern explanations

4. **Best Practices**
   - Test naming conventions
   - Encapsulation patterns
   - Error handling strategies
   - Mock verification techniques

### Key Takeaways for Learning:

✅ **Tests document behavior** - Read test names to understand what code does
✅ **AAA pattern** - Every test: setup, execute, verify
✅ **Mocks isolate code** - Test one thing at a time
✅ **Async is important** - Modern C# uses async operations
✅ **Edge cases matter** - Test success AND failure scenarios
✅ **Names matter** - Good names make code self-documenting

---

## Next Steps for Your Learning:

1. **Run these tests** - See them pass/fail
2. **Modify tests** - Change assertions to see failures
3. **Write new tests** - Add tests for new scenarios
4. **Break the code** - Change mock returns, see how tests catch bugs
5. **Read real code** - These patterns apply everywhere

Good luck with your C# learning journey! 🚀

