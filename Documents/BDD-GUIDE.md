# BDD Approach to Testing

## What is BDD (Behavior-Driven Development)?

BDD is a development methodology that focuses on the behavior of the system from the user's perspective. Tests are written in the **Given-When-Then** format (Given-When-Then), which makes them understandable to all project participants.

## BDD Test Structure

### Given (Given) - Arrange
Describes the initial state - what conditions must exist before an action is performed.

```csharp
// Given: resident created with activation token
var resident = new ResidentAccount
{
    Id = Guid.NewGuid(),
    Email = "resident@example.com",
    IsActive = false,
    ActivationToken = "valid-token-123"
};
```

### When (When) - Act
Describes the action or event that occurs.

```csharp
// When: resident activates their account with valid password
var result = await residentService.ActivateAccountAsync(resident.Id, "SecurePassword123!");
```

### Then (Then) - Assert
Describes the expected result of the action.

```csharp
// Then: account becomes active
result.Should().BeTrue();
resident.IsActive.Should().BeTrue();
```

## Scenario Classification

### 1. Happy Path (Successful Scenario)

Describes how the system should work under ideal conditions.

```csharp
[Fact(DisplayName = "US-9: Successful booking confirmation")]
public async Task SuccessfulBookingConfirmation_WhenSlotSelected_BookingCreated()
{
    // Given: resident selected available slot
    var timeSlotId = Guid.NewGuid();
    var residentId = Guid.NewGuid();
    
    // When: resident confirms booking
    var booking = await bookingService.ConfirmBookingAsync(
        new CreateBookingRequest { ResidentId = residentId, TimeSlotId = timeSlotId }
    );
    
    // Then: booking successfully created
    booking.Should().NotBeNull();
    booking.Status.Should().Be("Confirmed");
}
```

### 2. Negative Path (Negative Scenario)

Describes how the system should handle errors and edge cases.

```csharp
[Fact(DisplayName = "US-10: Attempt to double book the same slot")]
public async Task BookingFails_WhenSlotAlreadyBooked_ErrorThrown()
{
    // Given: slot already reserved by another resident
    var timeSlotId = Guid.NewGuid();
    
    // When: another resident tries to reserve the same slot
    var bookingRequest = new CreateBookingRequest { TimeSlotId = timeSlotId };
    
    // Then: system denies with error
    await Assert.ThrowsAsync<InvalidOperationException>(
        () => bookingService.ConfirmBookingAsync(bookingRequest)
    );
}
```

### 3. Edge Cases (Edge Cases)

Describes behavior at the boundaries of the system.

```csharp
[Fact(DisplayName = "US-14: Attempt to cancel already started booking")]
public async Task CancellationDenied_WhenBookingHasStarted()
{
    // Given: booking already started
    var booking = new Booking { StartTime = DateTime.UtcNow.AddMinutes(-10) };
    
    // When: resident tries to cancel it
    var canCancel = await bookingService.CanCancelBookingAsync(booking.Id);
    
    // Then: system forbids cancellation
    canCancel.Should().BeFalse();
}
```

## Test Naming

Follow this format:

```
[Action]_[Condition]_[ExpectedResult]
or
[Action]_When[Condition]_Then[ExpectedResult]
```

### Examples of Well-Named Tests:
- ✅ `ActivateAccount_WithValidToken_AccountBecomesActive`
- ✅ `LoginFails_WhenAccountInactive_ErrorMessageShown`
- ✅ `RefreshBookingCalendar_AfterSlotBooked_UpdatedAvailabilityShown`
- ✅ `SendConfirmationEmail_WhenBookingConfirmed_EmailDelivered`

### Examples of Poorly Named Tests:
- ❌ `Test1()`
- ❌ `ShouldWork()`
- ❌ `TestActivationAndLogin()`

## Mocking in BDD

Mocking allows you to isolate a unit of code and test it independently.

### Example with Moq:

```csharp
public class BookingConfirmationTests
{
    private readonly Mock<IBookingService> _bookingServiceMock;
    private readonly Mock<IEmailService> _emailServiceMock;

    [Fact(DisplayName = "US-22: Confirmation received when booking successfully confirmed")]
    public async Task ConfirmationEmailSent_WhenBookingConfirmed()
    {
        // Arrange
        var bookingRequest = new CreateBookingRequest { ... };
        var expectedBooking = new Booking { Status = "Confirmed" };

        // Setup mocked service
        _bookingServiceMock
            .Setup(x => x.ConfirmBookingAsync(bookingRequest))
            .ReturnsAsync(expectedBooking);

        _emailServiceMock
            .Setup(x => x.SendBookingConfirmationAsync(It.IsAny<BookingConfirmationEmail>()))
            .ReturnsAsync(true);

        // Act
        var booking = await _bookingServiceMock.Object.ConfirmBookingAsync(bookingRequest);
        var emailSent = await _emailServiceMock.Object.SendBookingConfirmationAsync(
            new BookingConfirmationEmail { ... }
        );

        // Assert
        booking.Status.Should().Be("Confirmed");
        emailSent.Should().BeTrue();

        // Verify: check that method was called
        _emailServiceMock.Verify(
            x => x.SendBookingConfirmationAsync(It.IsAny<BookingConfirmationEmail>()),
            Times.Once
        );
    }
}
```

## Assertions with FluentAssertions

FluentAssertions provides readable and intuitive assertions.

### Examples:

```csharp
// Boolean values
result.Should().BeTrue();
result.Should().BeFalse();

// Null check
resident.Should().NotBeNull();
resident.Should().BeNull();

// Equality
booking.Status.Should().Be("Confirmed");
booking.ResidentId.Should().Be(expectedResidentId);

// Collections
bookings.Should().HaveCount(3);
bookings.Should().BeEmpty();
bookings.Should().Contain(b => b.IsMyBooking);
bookings.Should().NotContain(b => b.IsBooked);

// Strings
email.Should().Contain("@");
email.Should().StartWith("resident");
message.Should().NotBeNullOrEmpty();

// Exceptions
await Assert.ThrowsAsync<InvalidOperationException>(
    () => service.MethodAsync()
);
```

## Organizing Tests into Classes

Group related tests by functionality:

```csharp
// ✅ Good: one class per feature
public class ResidentAccountActivationTests { }
public class ResidentLoginTests { }
public class AdminLoginTests { }

// ❌ Bad: mixing different features
public class AuthenticationTests { } // Too generic
```

## Integration Tests

Unlike unit tests, integration tests verify the interaction of multiple components.

```csharp
[Fact(DisplayName = "Integration: Account creation → Activation → Login")]
public async Task CompleteAccountFlow_FromCreationToLogin()
{
    // 1. Admin creates account
    var resident = await adminService.CreateResidentAsync(createRequest);
    resident.IsActive.Should().BeFalse();

    // 2. Activation email sent
    var emailSent = await emailService.SendActivationEmailAsync(resident.Email, ...);
    emailSent.Should().BeTrue();

    // 3. Resident activates account
    var activated = await residentService.ActivateAccountAsync(resident.Id, password);
    activated.Should().BeTrue();

    // 4. Resident can login
    var loginResult = await residentService.LoginAsync(resident.ApartmentNumber, password);
    loginResult.Success.Should().BeTrue();
}
```

## Data Builders for Test Data

Use builders to create test objects:

```csharp
// Instead of:
var resident = new ResidentAccount 
{ 
    Id = Guid.NewGuid(),
    Name = "Test",
    Email = "test@example.com",
    // ... 10 fields
};

// Use:
var resident = ResidentTestDataBuilder.BuildValidResident();
```

## Parametrized Tests

For testing the same logic with different data:

```csharp
[Theory]
[InlineData("short")]
[InlineData("")]
[InlineData(null)]
[DisplayName = "US-20: Error when validating password"]
public async Task PasswordValidationFails_WithInvalidPasswords(string password)
{
    // Test logic that applies to all data
    var result = await ValidatePasswordAsync(password);
    result.Should().BeFalse();
}
```

## Special Scenarios for Testing

### 1. Asynchronous Operations
```csharp
[Fact]
public async Task AsyncOperation_CompletesSuccessfully()
{
    var result = await asyncService.DoSomethingAsync();
    result.Should().NotBeNull();
}
```

### 2. Timeouts
```csharp
[Fact]
public async Task OperationTimeout_WhenDelayed()
{
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
    
    await Assert.ThrowsAsync<OperationCanceledException>(
        () => slowService.SlowOperationAsync(cts.Token)
    );
}
```

### 3. Database State
```csharp
[Fact]
public async Task BookingCreated_InDatabase()
{
    // Act
    var booking = await bookingService.CreateBookingAsync(request);
    
    // Assert - check in DB
    var storedBooking = await database.Bookings.FirstAsync(b => b.Id == booking.Id);
    storedBooking.Should().NotBeNull();
    storedBooking.Status.Should().Be("Confirmed");
}
```

## BDD Best Practices

1. **One Assert per 'Then' section**
   ```csharp
   // ❌ Bad
   result.Should().BeTrue();
   result.Status.Should().Be("Active");
   result.Email.Should().Be("test@example.com");
   
   // ✅ Good - one fact per test or use BeEquivalentTo
   result.Should().BeEquivalentTo(new { Status = "Active", Email = "test@example.com" });
   ```

2. **Cleanup after tests**
   ```csharp
   public void Dispose()
   {
       // Clean up DB, temp files, etc
   }
   ```

3. **Use DisplayName**
   ```csharp
   [Fact(DisplayName = "US-9: Successful booking confirmation when slot selected")]
   public async Task Test() { }
   ```

4. **Don't rely on test execution order**
   ```csharp
   // Each test should be independent
   [Fact]
   public async Task Test1() { }
   
   [Fact]
   public async Task Test2() { }
   // Test2 should NOT depend on Test1 result
   ```

5. **Test edge cases**
   ```csharp
   [Theory]
   [InlineData(DateTime.UtcNow.AddMinutes(-1))]  // Past times
   [InlineData(DateTime.UtcNow.AddYears(10))]    // Far future
   public async Task EdgeCases(DateTime date) { }
   ```

## Example of Complete BDD Test

```csharp
[Fact(DisplayName = "US-14: Successful cancellation of booking within allowed time")]
public async Task CancelBooking_WhenWithinCancellationWindow_BookingCancelled()
{
    // GIVEN: Resident has confirmed booking on future date
    //        more than 24 hours before start
    var residentId = Guid.NewGuid();
    var bookingId = Guid.NewGuid();
    var futureBooking = new Booking
    {
        Id = bookingId,
        ResidentId = residentId,
        StartTime = DateTime.UtcNow.AddDays(2), // In 2 days
        Status = "Confirmed"
    };
    
    _bookingServiceMock
        .Setup(x => x.GetBookingAsync(bookingId))
        .ReturnsAsync(futureBooking);
    
    _bookingServiceMock
        .Setup(x => x.CanCancelBookingAsync(bookingId))
        .ReturnsAsync(true);

    // WHEN: Resident clicks "Cancel Booking" button
    var result = await _bookingServiceMock.Object.CancelBookingAsync(bookingId, residentId);

    // THEN: Booking is cancelled,
    //       time slot becomes available again,
    //       resident receives confirmation
    result.Should().BeTrue();
    
    _bookingServiceMock.Verify(
        x => x.CancelBookingAsync(bookingId, residentId),
        Times.Once
    );
}
```

## Dependencies and Testing

### Dependencies are best tested through interfaces:

```csharp
public interface IBookingService
{
    Task<Booking> ConfirmBookingAsync(CreateBookingRequest request);
    Task<bool> CancelBookingAsync(Guid bookingId, Guid residentId);
}

// In test you mock the interface, not the concrete implementation
var mock = new Mock<IBookingService>();
```

## Running and Getting Reports

```bash
# Run all tests
dotnet test

# Run with details
dotnet test --verbosity detailed

# Generate code coverage report
dotnet test /p:CollectCoverage=true
```
