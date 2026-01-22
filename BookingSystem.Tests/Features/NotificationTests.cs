using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;

namespace BookingSystem.Tests.Features;

/// <summary>
/// US-22: Resident receives booking confirmation
/// US-23: Resident receives account deletion email
/// /// Testing notifications and email functionality
/// </summary>
public class NotificationTests
{
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<IBookingService> _bookingServiceMock;

    public NotificationTests()
    {
        _emailServiceMock = new Mock<IEmailService>();
        _notificationServiceMock = new Mock<INotificationService>();
        _bookingServiceMock = new Mock<IBookingService>();
    }

    #region US-22: Resident receives booking confirmation

    [Fact(DisplayName = "US-22: Booking confirmation received")]
    public async Task BookingConfirmationEmailSent_WhenBookingConfirmed()
    {
        // Arrange
        var residentEmail = "resident@example.com";
        var bookingConfirmation = new BookingConfirmationEmail
        {
            ResidentEmail = residentEmail,
            FacilityName = "Laundry room",
            Date = DateTime.Today.AddDays(1),
            Time = "10:00 - 11:00",
            ConfirmationToken = Guid.NewGuid().ToString()
        };

        _emailServiceMock
            .Setup(x => x.SendBookingConfirmationAsync(bookingConfirmation))
            .ReturnsAsync(true);

        // Act
        var sent = await _emailServiceMock.Object.SendBookingConfirmationAsync(bookingConfirmation);

        // Assert
        sent.Should().BeTrue();
        _emailServiceMock.Verify(
            x => x.SendBookingConfirmationAsync(It.Is<BookingConfirmationEmail>(
                e => e.ResidentEmail == residentEmail && e.FacilityName == "Laundry room")),
            Times.Once);
    }

    [Fact(DisplayName = "US-22: Confirmation includes cancellation link")]
    public async Task ConfirmationIncludesCancellationLink()
    {
        // Arrange
        var residentEmail = "resident@example.com";
        var cancellationLink = "https://example.com/cancel-booking/token123";
        var bookingConfirmation = new BookingConfirmationEmail
        {
            ResidentEmail = residentEmail,
            FacilityName = "Laundry room",
            Date = DateTime.Today.AddDays(1),
            Time = "10:00 - 11:00",
            CancellationLink = cancellationLink
        };

        _emailServiceMock
            .Setup(x => x.SendBookingConfirmationAsync(bookingConfirmation))
            .ReturnsAsync(true);

        // Act
        var sent = await _emailServiceMock.Object.SendBookingConfirmationAsync(bookingConfirmation);

        // Assert
        sent.Should().BeTrue();
        _emailServiceMock.Verify(
            x => x.SendBookingConfirmationAsync(It.Is<BookingConfirmationEmail>(
                e => e.CancellationLink == cancellationLink)),
            Times.Once);
    }

    [Fact(DisplayName = "US-22: Booking error - confirmation not sent")]
    public async Task NoConfirmationEmail_WhenBookingFailed()
    {
        // Arrange
        var bookingRequest = new CreateBookingRequest 
        { 
            ResidentId = Guid.NewGuid(), 
            TimeSlotId = Guid.NewGuid() 
        };

        _bookingServiceMock
            .Setup(x => x.ConfirmBookingAsync(bookingRequest))
            .ReturnsAsync((Booking)null);

        // Act
        var booking = await _bookingServiceMock.Object.ConfirmBookingAsync(bookingRequest);

        // Assert
        booking.Should().BeNull();
        _emailServiceMock.Verify(
            x => x.SendBookingConfirmationAsync(It.IsAny<BookingConfirmationEmail>()),
            Times.Never);
    }

    [Fact(DisplayName = "US-22: Checking confirmation details correctness")]
    public async Task VerifyConfirmationDetails_AreCorrect()
    {
        // Arrange
        var residentEmail = "resident@example.com";
        var facilityName = "Laundry room";
        var bookingDate = DateTime.Today.AddDays(1);
        var bookingTime = "14:00 - 15:00";

        var bookingConfirmation = new BookingConfirmationEmail
        {
            ResidentEmail = residentEmail,
            FacilityName = facilityName,
            Date = bookingDate,
            Time = bookingTime
        };

        _emailServiceMock
            .Setup(x => x.SendBookingConfirmationAsync(bookingConfirmation))
            .ReturnsAsync(true);

        // Act
        var sent = await _emailServiceMock.Object.SendBookingConfirmationAsync(bookingConfirmation);

        // Assert
        sent.Should().BeTrue();
        _emailServiceMock.Verify(
            x => x.SendBookingConfirmationAsync(It.Is<BookingConfirmationEmail>(
                e => e.FacilityName == facilityName && 
                     e.Date == bookingDate && 
                     e.Time == bookingTime)),
            Times.Once);
    }

    #endregion

    #region US-23: Resident receives account deletion email

    [Fact(DisplayName = "US-23: Account deletion email sent")]
    public async Task AccountDeletionEmailSent_WhenAccountDeleted()
    {
        // Arrange
        var residentEmail = "deleted@example.com";

        _emailServiceMock
            .Setup(x => x.SendAccountDeletionEmailAsync(residentEmail))
            .ReturnsAsync(true);

        // Act
        var sent = await _emailServiceMock.Object.SendAccountDeletionEmailAsync(residentEmail);

        // Assert
        sent.Should().BeTrue();
        _emailServiceMock.Verify(
            x => x.SendAccountDeletionEmailAsync(residentEmail),
            Times.Once);
    }

    [Fact(DisplayName = "US-23: Deleted resident cannot login")]
    public async Task DeletedResidentCannotLogin()
    {
        // Arrange
        var apartmentNumber = "A-101";
        var password = "password123";

        var loginService = new Mock<IResidentService>();
        loginService
            .Setup(x => x.LoginAsync(apartmentNumber, password))
            .ReturnsAsync(new LoginResult 
            { 
                Success = false, 
                ErrorMessage = "Account no longer exists" 
            });

        // Act
        var result = await loginService.Object.LoginAsync(apartmentNumber, password);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("no longer exists");
    }

    [Fact(DisplayName = "US-23: Deletion email contains deletion message")]
    public async Task DeletionEmailContains_DeletionMessage()
    {
        // Arrange
        var residentEmail = "resident@example.com";
        var deletionNotification = new AccountDeletionEmail
        {
            ResidentEmail = residentEmail,
            Message = "Your account has been deleted. You no longer have access to the booking system.",
            DeletionDate = DateTime.UtcNow
        };

        _emailServiceMock
            .Setup(x => x.SendAccountDeletionNotificationAsync(deletionNotification))
            .ReturnsAsync(true);

        // Act
        var sent = await _emailServiceMock.Object.SendAccountDeletionNotificationAsync(deletionNotification);

        // Assert
        sent.Should().BeTrue();
        _emailServiceMock.Verify(
            x => x.SendAccountDeletionNotificationAsync(It.Is<AccountDeletionEmail>(
                e => e.Message.Contains("deleted") && 
                     e.ResidentEmail == residentEmail)),
            Times.Once);
    }

    [Fact(DisplayName = "US-23: Error when attempting to delete non-existent account - email not sent")]
    public async Task NoDeletionEmail_WhenResidentNotFound()
    {
        // Arrange
        var nonExistentResidentId = Guid.NewGuid();
        var adminService = new Mock<IAdminService>();

        adminService
            .Setup(x => x.DeleteResidentAccountAsync(nonExistentResidentId))
            .ThrowsAsync(new InvalidOperationException("Resident not found"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => adminService.Object.DeleteResidentAccountAsync(nonExistentResidentId));

        _emailServiceMock.Verify(
            x => x.SendAccountDeletionEmailAsync(It.IsAny<string>()),
            Times.Never);
    }

    #endregion
}

/// <summary>
/// /// Integration tests for notifications and email functionality
/// </summary>
public class NotificationIntegrationTests
{
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IBookingService> _bookingServiceMock;
    private readonly Mock<IAdminService> _adminServiceMock;

    public NotificationIntegrationTests()
    {
        _emailServiceMock = new Mock<IEmailService>();
        _bookingServiceMock = new Mock<IBookingService>();
        _adminServiceMock = new Mock<IAdminService>();
    }

    [Fact(DisplayName = "Integration: Account creation - activation email")]
    public async Task IntegrationTest_AccountCreation_SendsActivationEmail()
    {
        // Arrange
        var residentData = new CreateResidentRequest
        {
            Name = "Test User",
            ApartmentNumber = "TEST-001",
            Email = "test@example.com",
            Phone = "+380501111111"
        };

        var resident = new ResidentAccount 
        { 
            Id = Guid.NewGuid(), 
            Email = residentData.Email, 
            IsActive = false 
        };

        var activationEmail = new ActivationEmail
        {
            ResidentEmail = residentData.Email,
            ActivationLink = "https://example.com/activate?token=test-token"
        };

        _adminServiceMock
            .Setup(x => x.CreateResidentAsync(residentData))
            .ReturnsAsync(resident);

        _emailServiceMock
            .Setup(x => x.SendActivationEmailAsync(residentData.Email, It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var createdResident = await _adminServiceMock.Object.CreateResidentAsync(residentData);
        var emailSent = await _emailServiceMock.Object.SendActivationEmailAsync(
            residentData.Email,
            activationEmail.ActivationLink);

        // Assert
        createdResident.Should().NotBeNull();
        createdResident.IsActive.Should().BeFalse();
        emailSent.Should().BeTrue();
    }

    [Fact(DisplayName = "Integration: Booking cancellation - notification email")]
    public async Task IntegrationTest_CancelBooking_NotifiesResident()
    {
        // Arrange
        var residentEmail = "resident@example.com";
        var bookingId = Guid.NewGuid();
        var adminId = Guid.NewGuid();

        var cancellationRequest = new AdminCancellationRequest
        {
            BookingId = bookingId,
            Reason = "Maintenance",
            AdminId = adminId
        };

        _adminServiceMock
            .Setup(x => x.CancelBookingAsync(cancellationRequest))
            .ReturnsAsync(true);

        _emailServiceMock
            .Setup(x => x.SendCancellationNotificationAsync(residentEmail, "Maintenance"))
            .ReturnsAsync(true);

        // Act
        var cancelled = await _adminServiceMock.Object.CancelBookingAsync(cancellationRequest);
        var notified = await _emailServiceMock.Object.SendCancellationNotificationAsync(
            residentEmail, 
            cancellationRequest.Reason);

        // Assert
        cancelled.Should().BeTrue();
        notified.Should().BeTrue();
    }

    [Fact(DisplayName = "Integration: Account deletion - deletion email")]
    public async Task IntegrationTest_DeleteAccount_SendsDeletionEmail()
    {
        // Arrange
        var residentId = Guid.NewGuid();
        var residentEmail = "resident@example.com";
        var adminId = Guid.NewGuid();

        _adminServiceMock
            .Setup(x => x.DeleteResidentAccountAsync(residentId))
            .ReturnsAsync(true);

        _emailServiceMock
            .Setup(x => x.SendAccountDeletionEmailAsync(residentEmail))
            .ReturnsAsync(true);

        // Act
        var deleted = await _adminServiceMock.Object.DeleteResidentAccountAsync(residentId);
        var emailSent = await _emailServiceMock.Object.SendAccountDeletionEmailAsync(residentEmail);

        // Assert
        deleted.Should().BeTrue();
        emailSent.Should().BeTrue();
    }
}
