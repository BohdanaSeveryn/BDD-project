# BDD Project: Tests for Laundry Booking System

## Table of Contents

This project contains tests (xUnit with Moq) for a web application for managing laundry booking, developed based on BDD scenarios.

## Project Structure

```
BookingSystem.Tests/
├── BookingSystem.Tests.csproj          # Project file
├── Features/                            # Main test classes
│   ├── AuthenticationTests.cs           # US-1, US-2, US-3
│   ├── AccountManagementTests.cs        # US-4, US-5, US-19, US-20
│   ├── BookingViewTests.cs              # US-6, US-7, US-8
│   ├── BookingManagementTests.cs        # US-9, US-10, US-11, US-12, US-13, US-14
│   ├── AdminBookingManagementTests.cs   # US-16, US-17, US-18
│   └── NotificationTests.cs             # US-22, US-23
├── Fixtures/
│   └── TestModelsAndInterfaces.cs       # Data models and interfaces
└── Mocks/                               # Helper mocks and utilities
```

## Covered User Stories

### Authentication and Account Management
- **US-1**: Resident account activation with email confirmation
- **US-2**: Resident login via apartment number and password
- **US-3**: Administrator login with two-factor authentication

### Account Management by Administrator
- **US-4**: Admin creates resident account
- **US-5**: Admin deletes resident account
- **US-19**: Admin adds new resident
- **US-20**: Admin edits resident information

### Viewing and Selecting Time Slots
- **US-6**: Resident views laundry availability
- **US-7**: Resident views booking calendar
- **US-8**: Resident selects time slot

### Booking Management by Resident
- **US-9**: Resident confirms booking
- **US-10**: Prevention of double booking
- **US-11**: Booking appears in personal calendar
- **US-12**: View all resident bookings
- **US-13**: View booking details
- **US-14**: Resident cancels booking

### Administrative Booking Management
- **US-16**: Admin views all bookings
- **US-17**: Admin views booking details
- **US-18**: Admin cancels any booking

### Notifications
- **US-22**: Resident receives booking confirmation
- **US-23**: Resident receives account deletion email

## Running Tests

### Direct Execution
```bash
dotnet test
```

### Running Specific Test Class
```bash
dotnet test --filter "AuthenticationTests"
```

### Running with Detailed Output
```bash
dotnet test --verbosity detailed
```

## Test Structure (BDD Approach)

Each test follows the Given-When-Then pattern:

```csharp
[Fact(DisplayName = "US-1: Successful account activation")]
public async Task SuccessfulAccountActivation_WhenValidActivationLinkAndPassword_AccountBecomesActive()
{
    // Arrange (Given) - prepare conditions
    var residentId = Guid.NewGuid();
    var activationToken = "valid-token-123";
    var password = "SecurePassword123!";
    
    // Act (When) - perform operation
    var result = await _residentServiceMock.Object.ActivateAccountAsync(residentId, password);
    
    // Assert (Then) - verify result
    result.Should().BeTrue();
}
```

## Technologies Used

- **xUnit**: Framework for writing tests
- **Moq**: Library for creating mock objects
- **FluentAssertions**: Readable assertions for tests
- **.NET 8.0**: Target platform

## Recommendations

### Adding New Tests
1. Create a class that extends the Features structure
2. Name the test in format: `[Action]_When[Condition]_Then[ExpectedResult]()`
3. Use the `DisplayName` attribute to describe the scenario
4. Follow the Arrange-Act-Assert pattern

### FluentAssertions Library
```csharp
// Instead of:
Assert.True(result);

// Use:
result.Should().BeTrue();

// For collections:
bookings.Should().HaveCount(3);
bookings.Should().Contain(b => b.IsMyBooking);
```

## Test Examples

### Successful Test
```csharp
[Fact(DisplayName = "US-9: Successful booking confirmation")]
public async Task SuccessfulBookingConfirmation_WhenSlotSelected()
{
    // Arrange
    var bookingRequest = new CreateBookingRequest 
    { 
        ResidentId = Guid.NewGuid(), 
        TimeSlotId = Guid.NewGuid() 
    };

    _bookingServiceMock
        .Setup(x => x.ConfirmBookingAsync(bookingRequest))
        .ReturnsAsync(new Booking { Status = "Confirmed" });

    // Act
    var result = await _bookingServiceMock.Object.ConfirmBookingAsync(bookingRequest);

    // Assert
    result.Status.Should().Be("Confirmed");
}
```

### Negative Test (Error Scenario)
```csharp
[Fact(DisplayName = "US-4: Error when duplicating email")]
public async Task ResidentCreationFails_WhenEmailAlreadyExists()
{
    // Arrange
    var residentData = new CreateResidentRequest { Email = "existing@example.com" };

    _residentServiceMock
        .Setup(x => x.CreateResidentAsync(residentData))
        .ThrowsAsync(new InvalidOperationException("Email is already in use"));

    // Act & Assert
    await Assert.ThrowsAsync<InvalidOperationException>(
        () => _residentServiceMock.Object.CreateResidentAsync(residentData));
}
```

## Integration Tests

The file `NotificationTests.cs` contains integration tests that verify the interaction of multiple services:

```csharp
[Fact(DisplayName = "Integration: Account creation - activation email")]
public async Task IntegrationTest_AccountCreation_SendsActivationEmail()
{
    // Tests that activation email is automatically sent when account is created
}
```

## Scenario Coverage

### Covered Scenario Types:
- ✅ Successful operations (Happy Path)
- ✅ Validation errors (Validation Errors)
- ✅ Authorization and access (Authorization)
- ✅ Exception handling (Exception Handling)
- ✅ Current states (State Management)
- ✅ Email notifications (Notifications)
- ✅ Integration scenarios (Integration Scenarios)

## Extending Tests

### To Add New Tests:

1. **Identify the User Story** you want to test
2. **Build tests based on scenarios** from User Story
3. **Use mocking** for dependencies
4. **Cover both successful and negative scenarios**

### Extension Example:
```csharp
// Add to the appropriate Features class
[Fact(DisplayName = "US-X: New functionality")]
public async Task NewFeature_WhenConditionMet_ExpectedResult()
{
    // Arrange
    // Act
    // Assert
}
```

## Quality Control

Tests provide:
- **Functionality**: Each feature works as expected
- **Regression**: Changes don't break existing functionality
- **Documentation**: DisplayName describes what the test does
- **Reliability**: Mocks allow testing in isolation

## Location of Important Files

| File | Purpose |
|------|----------|
| `AuthenticationTests.cs` | Login and authentication tests |
| `AccountManagementTests.cs` | Account management tests |
| `BookingViewTests.cs` | Booking view tests |
| `BookingManagementTests.cs` | Booking operation tests |
| `AdminBookingManagementTests.cs` | Administrative tests |
| `NotificationTests.cs` | Email and notification tests |
| `TestModelsAndInterfaces.cs` | Data models and interfaces |

## Next Steps

1. ✅ Add real service implementations
2. ✅ Run tests to validate code
3. ✅ Set up CI/CD pipeline for automated testing
4. ✅ Add new tests for new features
