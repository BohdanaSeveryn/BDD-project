# Booking System Test Code Overview and Explanation

This document provides a comprehensive explanation of all C# test files in the BookingSystem.Tests project, detailing what each file does, its purpose, and how the code functions.

---

## Project Overview

The Booking System Test project is a Behavior-Driven Development (BDD) test suite for a resident facility booking management system. It uses **xUnit** for testing, **Moq** for mocking dependencies, and **FluentAssertions** for readable test assertions.

The system manages resident bookings for shared facilities (like laundry rooms) with user roles including residents and administrators.

---

## File Structure Overview

```
BookingSystem.Tests/
├── Features/                          # Feature test files organized by business capability
│   ├── AccountManagementTests.cs       # US-1, US-2, US-3: Account activation and login
│   ├── AdminBookingManagementTests.cs  # US-16, US-17, US-18: Admin booking operations
│   ├── AdminAccountManagementTests.cs  # US-4, US-5, US-19, US-20: Admin account management
│   ├── AuthenticationTests.cs          # US-1, US-2, US-3: Authentication flows
│   ├── BookingManagementTests.cs       # US-9 through US-14: Resident booking operations
│   ├── BookingViewTests.cs             # US-6, US-7, US-8: Viewing bookings and availability
│   └── NotificationTests.cs            # US-22, US-23: Email notifications
└── Fixtures/                          # Shared test utilities and mock data
    ├── TestDataBuilders.cs            # Helper classes for building test data
    └── TestModelsAndInterfaces.cs     # Mock service interfaces and data models
```

---

## Detailed File Explanations

### 1. AuthenticationTests.cs
**Location:** [BookingSystem.Tests/Features/AuthenticationTests.cs](BookingSystem.Tests/Features/AuthenticationTests.cs)

**Purpose:** Tests authentication flows for residents and administrators, including account activation and login with two-factor authentication.

**What It Does:**

#### Class: ResidentAccountActivationTests
Tests the account activation process for new residents:
- **SuccessfulAccountActivation_WhenValidActivationLinkAndPassword_AccountBecomesActive**: Verifies that a resident can activate their account by providing a valid activation link and password. The account transitions from inactive to active status.
- **ActivationEmailSent_WhenAccountCreated**: Confirms that when a new resident account is created, an activation email is automatically sent with an activation link.
- **ActivationLinkExpired_WhenTokenExpiryTimeReached**: Tests that activation tokens have an expiration time. If a resident tries to activate after the token expires, the activation fails.
- **LoginFails_WhenAccountNotActivated**: Ensures that residents cannot log in before activating their account.

#### Class: ResidentLoginTests
Tests the login process for residents:
- **SuccessfulLogin_WhenValidCredentialsAndAccountActive**: Verifies successful login when the apartment number and password are correct and the account is active. Returns a login result with a JWT token.
- **LoginFails_WhenAccountInactive**: Confirms that login is denied if the account hasn't been activated.
- **LoginFails_WhenInvalidCredentials**: Tests that login fails with an error message when incorrect credentials are provided.

#### Class: AdminLoginTests
Tests admin login with enhanced security (two-factor authentication):
- **SuccessfulAdminLogin_WhenCredentialsAndTwoFactorCodeValid**: Verifies that admin login succeeds when credentials are correct AND a valid two-factor code is provided.
- **TwoFactorCodeSent_WhenAdminAuthenticated**: Confirms that after successful initial authentication, a two-factor code is sent to the admin's email.
- **AdminAccessDenied_WhenTwoFactorCodeInvalid**: Tests that even with correct credentials, access is denied if the two-factor code is wrong.

**Key Patterns:**
- Uses mocks to simulate service calls without actual database or email service
- Tests both success and failure scenarios
- Validates the complete authentication flow end-to-end

---

### 2. AdminAccountManagementTests.cs
**Location:** [BookingSystem.Tests/Features/AdminAccountManagementTests.cs](BookingSystem.Tests/Features/AdminAccountManagementTests.cs)

**Purpose:** Tests administrative functions for managing resident accounts, including creation, deletion, and updating resident information.

**What It Does:**

#### Section: US-4 - Admin Creates Resident Account
- **SuccessfulResidentAccountCreation_WhenValidDataProvided**: Tests that an admin can create a new resident account with valid name, apartment number, email, and phone. The account is created in inactive state, and an activation email is sent.
- **ResidentCreationFails_WhenEmailAlreadyExists**: Validates that duplicate emails are prevented - the system throws an error if email already exists.
- **ResidentCreationFails_WhenPhoneAlreadyExists**: Tests that duplicate phone numbers are also prevented.
- **ResidentLoginFails_WhenAccountNotActivated**: Confirms that newly created accounts cannot be used to log in until activated.

#### Section: US-5 - Admin Deletes Resident Account
- **SuccessfulResidentAccountDeletion_WhenAdminAuthorized**: Tests successful account deletion, including sending a deletion confirmation email.
- **ResidentDeletionFails_WhenResidentNotFound**: Validates error handling when trying to delete a non-existent account.
- **DeletedResidentLoginFails_WhenAccountDeleted**: Confirms that deleted residents cannot log in anymore.

#### Section: US-19 - Admin Adds New Resident
- **NewResidentAdded_WhenValidDetailsProvided**: Very similar to US-4, tests adding a new resident with email validation.
- **ResidentCreationFails_WhenEmailMissing**: Tests validation that email is a required field.
- **ResidentCreationFails_WhenDuplicateEmail**: Tests duplicate email prevention.

#### Section: US-20 - Admin Edits Resident Information
- **ResidentInformationUpdated_WhenValidChanges**: Tests that an admin can update resident name, apartment number, and phone.
- **ResidentEmailUpdated_WhenNewEmailProvided**: Tests email updates with notification sent to new email.
- **ResidentUpdateFails_WhenNameEmpty**: Validates that name cannot be empty.
- **CancelEditingWithoutSaving_NoChangesApplied**: Tests that changes are not persisted if the resident cancels the edit.

**Key Patterns:**
- Tests both positive (success) and negative (error) scenarios
- Validates data constraints and business rules
- Tests notification side effects (emails sent)

---

### 3. AdminBookingManagementTests.cs
**Location:** [BookingSystem.Tests/Features/AdminBookingManagementTests.cs](BookingSystem.Tests/Features/AdminBookingManagementTests.cs)

**Purpose:** Tests administrative booking management capabilities including viewing all bookings, viewing details, and canceling bookings.

**What It Does:**

#### Section: US-16 - Admin Views All Bookings
- **AdminViewsBookingCalendar_AllSlots**: Tests that an admin can view a calendar of all booking slots (both available and booked) for a specific date.
- **FilterBookings_ByDateRange**: Tests filtering bookings by a date range (e.g., next 7 days).
- **EmptyCalendar_WhenNoBookings**: Tests that the calendar gracefully shows empty when there are no bookings.
- **ErrorLoadingBookings_ServerError**: Tests error handling when the booking service fails.
- **ViewCalendar_WithIncompleteData**: Tests resilience when some booking data is corrupted or incomplete.

#### Section: US-17 - Admin Views Booking Details
- **AdminViewsBookingDetails_WithResidentInfo**: Tests that admin can see full booking details including resident name, apartment, email, phone, date, and time.
- **ErrorLoadingBookingDetails_ServerError**: Tests error handling during detail retrieval.
- **IncompleteBookingData_ShowsWarning**: Tests that when resident data is missing, a warning is displayed instead of crashing.
- **BookingByRemovedResident_ShowsMessage**: Tests that if a resident deletes their account, their bookings show "[Deleted]" instead of a name.

#### Section: US-18 - Admin Cancels Booking
- **AdminCancelsBooking_ForMaintenance**: Tests that admin can cancel a booking and notify the resident with a reason (e.g., "Maintenance").
- **ResidentNotified_WhenBookingCancelled**: Confirms that residents receive email notification when their booking is canceled.
- **AdminProvidesCancellationReason_IncludedInNotification**: Tests that the cancellation reason provided by the admin is included in the resident notification.
- **ResidentCannotCancelOtherBooking_AccessDenied**: Tests that a resident cannot cancel another resident's booking (access control).

**Key Patterns:**
- Tests administrative privileges and authorization
- Tests notification flows
- Tests error handling for corrupted data
- Tests access control restrictions

---

### 4. BookingViewTests.cs
**Location:** [BookingSystem.Tests/Features/BookingViewTests.cs](BookingSystem.Tests/Features/BookingViewTests.cs)

**Purpose:** Tests resident views for checking facility availability and booking calendars, plus time slot selection functionality.

**What It Does:**

#### Section: US-6 - Resident Views Washing Machine Availability
- **ResidentSeesAvailableTimeSlots_WhenLogged**: Tests that an authenticated resident can see available time slots for a facility on a specific date. Slots show which are available (green) and which are booked (red/unavailable).
- **ResidentDistinguishesAvailableFromUnavailable**: Tests that each slot clearly indicates its status (Available vs Reserved).
- **UserRedirectedToLogin_WhenNotAuthenticated**: Tests that unauthenticated users cannot view availability and are redirected to login.
- **AvailabilityLoaded_WhenDifferentDateSelected**: Tests that availability can be loaded for different future dates.

#### Section: US-7 - Resident Views Booking Calendar
- **ResidentViewsCalendar_ForCurrentDay**: Tests viewing the calendar for today's available slots.
- **TimeSlotsProperlyCoded_GreenForAvailableRedForUnavailable**: Tests that slots are color-coded for visual distinction (green = available, red = reserved).
- **ViewAvailability_ForFutureDate**: Tests that availability can be viewed for dates weeks in the future.

#### Section: US-8 - Resident Selects Time Slot (TimeSlotSelectionTests)
- **SelectAvailableTimeSlot_SlotHighlighted**: Tests that when a resident clicks/selects an available slot, it becomes highlighted/selected.
- **ChangeSelection_FromFirstToSecondSlot**: Tests that a resident can change their selection from one slot to another before confirming.
- **SelectUnavailableSlot_FailsWithError**: Tests that attempting to select an already-booked slot fails with an error.

**Key Patterns:**
- Tests authorization (must be logged in)
- Tests data presentation (color coding, status indication)
- Tests interactive selection features
- Tests handling of past/future dates

---

### 5. BookingManagementTests.cs
**Location:** [BookingSystem.Tests/Features/BookingManagementTests.cs](BookingSystem.Tests/Features/BookingManagementTests.cs)

**Purpose:** Tests core booking operations including confirmation, preventing double-booking, calendar display, viewing details, and cancellation.

**What It Does:**

#### Section: US-9 - Resident Confirms Booking (BookingConfirmationTests)
- **SuccessfulBookingConfirmation_WhenSlotSelected**: Tests that when a resident confirms a selected time slot, a booking is created with "Confirmed" status.
- **CancelBeforeConfirmation_NoReservationCreated**: Tests that if a resident cancels their selection before confirming, no booking is created.
- **BookingFails_WhenServerError**: Tests error handling when the booking service fails.

#### Section: US-10 - Prevent Double-Booking
- **BookingFails_WhenSlotBookedDuringProcess**: Tests that if another resident books the same slot while the first resident is confirming, the booking fails with an appropriate error.
- **BookingRejected_WhenSlotAlreadyBooked**: Tests that attempting to book an already-reserved slot returns null/fails.
- **CalendarRefreshed_AfterDoubleBookingError**: Tests that after a double-booking error, the calendar is refreshed to show the slot as unavailable.

#### Section: US-11 - Booking Appears in My Calendar
- **PersonalBookingDisplayed_InCalendar**: Tests that after confirming a booking, it appears in the resident's personal calendar with correct details.
- **DistinguishPersonalFromOtherBookings**: Tests that the calendar shows which bookings belong to the current resident ("My booking") vs others' bookings.
- **ViewBookingDetails_OnHover**: Tests that hovering over a booking shows its details (date, time, whether it can be managed).

#### Section: US-12 - Resident Views All Bookings (BookingListViewTests)
- **ResidentViewsAllUpcomingBookings**: Tests that a resident can see a list of all their upcoming bookings.
- **NoBookingsMessage_WhenNoUpcomingBookings**: Tests that a friendly message is shown when the resident has no upcoming bookings.
- **RedirectToLogin_WhenNotAuthenticated**: Tests that unauthenticated users cannot view bookings.

#### Section: US-13 - Resident Views Booking Details
- **ViewBookingDetails_WhenBookingExists**: Tests that detailed information about a booking can be viewed (facility, date, time, cancellation option).
- **ViewDetailsOfMultipleBookings_Individually**: Tests that details for multiple bookings can be viewed one by one.
- **BookingDetailsNotFound_WhenBookingDeleted**: Tests error handling when a booking no longer exists.
- **ErrorLoadingBookingDetails_WhenServerError**: Tests server error handling.

#### Section: US-14 - Resident Cancels Booking (BookingCancellationTests)
- **CancelBooking_FromCalendarView**: Tests that a resident can cancel their booking from the calendar view.
- **CancelationDenied_AfterCancellationWindow**: Tests that bookings cannot be canceled after a certain time window (e.g., must cancel 24 hours before).
- **CancellationFails_WhenServerError**: Tests error handling.
- **CannotCancelStartedBooking**: Tests that bookings that have already started cannot be canceled.

**Key Patterns:**
- Tests complete booking lifecycle (select → confirm → view → cancel)
- Tests concurrency issues (double-booking prevention)
- Tests business rules (cancellation windows, access control)
- Tests calendar functionality and data display

---

### 6. NotificationTests.cs
**Location:** [BookingSystem.Tests/Features/NotificationTests.cs](BookingSystem.Tests/Features/NotificationTests.cs)

**Purpose:** Tests email notification functionality for bookings and account management.

**What It Does:**

#### Section: US-22 - Resident Receives Booking Confirmation
- **BookingConfirmationEmailSent_WhenBookingConfirmed**: Tests that when a booking is confirmed, a confirmation email is sent to the resident.
- **ConfirmationIncludesCancellationLink**: Tests that the confirmation email includes a link to cancel the booking.
- **NoConfirmationEmail_WhenBookingFailed**: Tests that no email is sent if booking confirmation fails.
- **VerifyConfirmationDetails_AreCorrect**: Tests that the email contains correct booking details (facility, date, time).

#### Section: US-23 - Resident Receives Account Deletion Email
- **AccountDeletionEmailSent_WhenAccountDeleted**: Tests that deletion confirmation is sent when an admin deletes a resident account.
- **DeletedResidentCannotLogin**: Tests that the deleted resident cannot log in.
- **DeletionEmailContains_DeletionMessage**: Tests that the deletion email contains an appropriate message.
- **NoDeletionEmail_WhenResidentNotFound**: Tests that no email is sent if the resident to delete doesn't exist.

#### Class: NotificationIntegrationTests
Tests end-to-end notification flows:
- **IntegrationTest_AccountCreation_SendsActivationEmail**: Tests that account creation triggers activation email.
- **IntegrationTest_CancelBooking_NotifiesResident**: Tests that booking cancellation triggers notification email.
- **IntegrationTest_DeleteAccount_SendsDeletionEmail**: Tests that account deletion triggers deletion email.

**Key Patterns:**
- Tests email triggering on business events
- Tests email content correctness
- Tests error scenarios (email not sent on failure)
- Tests integration between services

---

### 7. TestDataBuilders.cs
**Location:** [BookingSystem.Tests/Fixtures/TestDataBuilders.cs](BookingSystem.Tests/Fixtures/TestDataBuilders.cs)

**Purpose:** Provides helper methods to create consistent test data objects, following the Builder pattern.

**What It Does:**

#### Class: ResidentTestDataBuilder
Provides static methods to create resident-related test data:
- **BuildValidResident()**: Creates a complete ResidentAccount object with all required fields (name, email, phone, apartment, activation token, etc.)
- **BuildValidCreateRequest()**: Creates a CreateResidentRequest with valid resident creation data
- **BuildValidUpdateRequest()**: Creates an UpdateResidentRequest with valid update data

#### Class: BookingTestDataBuilder
Provides static methods for booking-related test data:
- **BuildDailyTimeSlots()**: Creates a list of 10 time slots covering a full business day (8 AM to 6 PM, with alternating available/unavailable slots)
- **BuildValidBooking()**: Creates a complete Booking object with resident and time slot references
- **BuildBookingConfirmationEmail()**: Creates a BookingConfirmationEmail with all required fields
- **BuildBookingDetails()**: Creates a BookingDetails object

#### Class: AdminTestDataBuilder
Provides static methods for admin-related test data:
- **BuildAdminBookingDetails()**: Creates an AdminBookingDetails object with resident and booking information
- **BuildCancellationRequest()**: Creates an AdminCancellationRequest
- **BuildMixedBookingItems()**: Creates a mixed list of booked and available calendar items

#### Class: BookingSystemFixture
A reusable fixture for tests that need shared state:
- Initializes common test IDs (resident, admin, facility)
- Sets up a test booking date
- Implements `IDisposable` for cleanup

#### Class: TestConstants
Centralizes all hard-coded test values:
- Valid and invalid email addresses
- Valid and invalid passwords and phone numbers
- Apartment numbers
- Activation and two-factor tokens
- Base URLs for links
- Cancellation reasons
- Time slot constants (morning, afternoon, evening)
- Error messages

#### Classes: ResidentAccountComparer and BookingComparer
Implements custom equality comparers for complex types:
- Used to compare test objects in assertions
- Define which properties constitute equality
- Provide hash codes for dictionary/set operations

**Key Patterns:**
- Centralized test data creation for consistency
- Reusable fixtures for test state
- Centralized constants for easy maintenance
- Custom comparers for complex object comparison

---

### 8. TestModelsAndInterfaces.cs
**Location:** [BookingSystem.Tests/Fixtures/TestModelsAndInterfaces.cs](BookingSystem.Tests/Fixtures/TestModelsAndInterfaces.cs)

**Purpose:** Defines all mock service interfaces and data model classes used throughout the tests.

**What It Does:**

#### Service Interfaces (For Mocking)

**IResidentService**: 
- Handles resident account operations
- Methods: ActivateAccountAsync, IsActivationTokenValidAsync, LoginAsync, GetResidentByIdAsync, CreateResidentAsync

**IAdminService**:
- Handles administrative operations
- Methods: IsAdminAuthorizedAsync, AuthenticateAsync, CreateResidentAsync, DeleteResidentAccountAsync, UpdateResidentAsync, GetAllBookingsAsync, GetBookingsByDateRangeAsync, GetBookingDetailsAsync, CancelBookingAsync

**IBookingService**:
- Core booking management
- Methods: GetAvailabilityAsync, SelectTimeSlotAsync, ClearSelectionAsync, ConfirmBookingAsync, RefreshAvailabilityAsync, GetMyBookingsAsync, GetCalendarBookingsAsync, GetBookingDetailsAsync, IsUserAuthorizedAsync, GetUpcomingBookingsAsync, CancelBookingAsync, CanCancelBookingAsync, CanResidentCancelBookingAsync

**IEmailService**:
- Handles email sending
- Methods: SendActivationEmailAsync, SendAccountDeletionEmailAsync, SendBookingConfirmationAsync, SendCancellationEmailAsync, SendAccountDeletionNotificationAsync, SendEmailChangeNotificationAsync, SendCancellationNotificationAsync

**ITwoFactorService**:
- Two-factor authentication
- Methods: SendTwoFactorCodeAsync, ValidateTwoFactorCodeAsync

**IAuthenticationService**:
- Basic authentication checks
- Methods: IsUserAuthenticatedAsync

**INotificationService**:
- Generic notification operations
- Methods: NotifyResidentOfCancellationAsync, SendCancellationEmailAsync

#### Data Model Classes

**ResidentAccount**: Represents a resident user
- Properties: Id, Name, Email, Phone, ApartmentNumber, IsActive, ActivationToken, ActivationTokenExpiry

**LoginResult**: Contains login operation results
- Properties: Success, ResidentId, Token, ErrorMessage

**CreateResidentRequest & UpdateResidentRequest**: Request DTOs for resident operations

**TimeSlot**: Represents a bookable time slot
- Properties: Id, StartTime, EndTime, IsAvailable, Status, Color

**Booking**: Represents a confirmed booking
- Properties: Id, ResidentId, TimeSlotId, Facility, Date, Time, StartTime, EndTime, DayOfWeek, IsMyBooking, Icon, Status, CreatedAt

**BookingSession**: Tracks current booking state
- Properties: SelectedSlotId

**BookingDetails & AdminBookingDetails**: Details view models for bookings

**BookingCalendarItem**: Calendar representation of a booking

**BookingConfirmationEmail**: Email content for booking confirmation

**AccountDeletionEmail & ActivationEmail**: Email content classes

**AdminCancellationRequest**: Admin booking cancellation request

**CreateBookingRequest**: Request to confirm a booking

**ValidationException**: Custom exception for validation errors

**Key Patterns:**
- Separates interfaces (contracts) from implementations for testing
- Models represent domain concepts
- Request/Response DTOs for data transfer
- Custom exceptions for specific error scenarios

---

## Key Technologies and Patterns

### Testing Framework: xUnit
- `[Fact]` attribute for individual test methods
- `[DisplayName("...")]` for readable test descriptions
- Supports async tests with `async Task`

### Mocking: Moq Library
- `Mock<IService>` creates mock implementations
- `.Setup()` configures mock behavior
- `.Verify()` asserts methods were called correctly
- `.ReturnsAsync()` for async method mocking
- `.ThrowsAsync()` for exception testing

### Assertions: FluentAssertions
- Readable assertions: `.Should().Be()`, `.Should().NotBeNull()`
- Collection assertions: `.Should().HaveCount()`, `.Should().Contain()`
- Boolean assertions: `.Should().BeTrue()`, `.Should().BeFalse()`

### Test Organization
- **AAA Pattern**: Arrange (setup), Act (execute), Assert (verify)
- **Sections**: Tests organized by user stories with `#region` blocks
- **Naming**: `[UnitOfWork]_[ScenarioUnderTest]_[ExpectedBehavior]()`

---

## Test Coverage Summary

| Component | Test Class | User Stories | Test Count |
|-----------|-----------|-------------|-----------|
| Authentication | AuthenticationTests | US-1, US-2, US-3 | 11 |
| Admin Accounts | AdminAccountManagementTests | US-4, US-5, US-19, US-20 | 14 |
| Admin Bookings | AdminBookingManagementTests | US-16, US-17, US-18 | 14 |
| Booking Views | BookingViewTests | US-6, US-7, US-8 | 9 |
| Booking Management | BookingManagementTests | US-9 through US-14 | 21 |
| Notifications | NotificationTests | US-22, US-23 | 11 |
| **TOTAL** | - | 23 | **80+** |

---

## How Tests Are Run

The project uses xUnit test framework. Tests can be run via:
- Visual Studio Test Explorer
- Command line: `dotnet test`
- CI/CD pipeline integration

Each test:
1. Initializes mock services in constructor
2. Sets up test data and mock behaviors in Arrange phase
3. Calls the method/service in Act phase
4. Verifies outcomes using assertions in Assert phase

---

## Key Business Logic Being Tested

1. **Account Lifecycle**: Creation → Activation → Login → Usage → Deletion
2. **Booking Lifecycle**: View Availability → Select Slot → Confirm → View → Cancel
3. **Permissions**: Residents can only manage their own bookings; Admins have elevated privileges
4. **Validation**: Email uniqueness, required fields, data format validation
5. **Notifications**: Email alerts for key business events
6. **Concurrency**: Prevention of double-booking with race condition handling
7. **Access Control**: Authentication and authorization checks
8. **Error Handling**: Graceful failure with appropriate error messages

---

## Conclusion

The BookingSystem.Tests project provides comprehensive test coverage of a facility booking management system. It demonstrates best practices in test organization, use of mocks and fixtures, and clear documentation of expected behavior through BDD-style test naming and organization. The tests ensure that users (residents and admins) can perform all required operations while maintaining data integrity, security, and appropriate notifications.

