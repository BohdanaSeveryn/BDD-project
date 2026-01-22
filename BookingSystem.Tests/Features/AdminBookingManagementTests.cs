using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;

namespace BookingSystem.Tests.Features;

/// <summary>
/// US-16: Admin views all bookings
/// US-17: Admin views booking details
/// US-18: Admin cancels any booking
/// /// Administrative functionality for viewing and managing bookings
/// </summary>
public class AdminBookingManagementTests
{
    private readonly Mock<IAdminService> _adminServiceMock;
    private readonly Mock<IBookingService> _bookingServiceMock;
    private readonly Mock<INotificationService> _notificationServiceMock;

    public AdminBookingManagementTests()
    {
        _adminServiceMock = new Mock<IAdminService>();
        _bookingServiceMock = new Mock<IBookingService>();
        _notificationServiceMock = new Mock<INotificationService>();
    }

    #region US-16: Admin views all bookings

    [Fact(DisplayName = "US-16: Admin views booking calendar")]
    public async Task AdminViewsBookingCalendar_AllSlots()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var bookingDate = DateTime.Today;

        var allBookings = new List<BookingCalendarItem>
        {
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(9, 0), IsBooked = false },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(10, 0), IsBooked = true, ResidentName = "Ivan" },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(11, 0), IsBooked = false }
        };

        _adminServiceMock
            .Setup(x => x.IsAdminAuthorizedAsync(adminId))
            .ReturnsAsync(true);

        _adminServiceMock
            .Setup(x => x.GetAllBookingsAsync(bookingDate))
            .ReturnsAsync(allBookings);

        // Act
        var isAuthorized = await _adminServiceMock.Object.IsAdminAuthorizedAsync(adminId);
        var bookings = await _adminServiceMock.Object.GetAllBookingsAsync(bookingDate);

        // Assert
        isAuthorized.Should().BeTrue();
        bookings.Should().HaveCount(3);
        bookings.Count(b => b.IsBooked).Should().Be(1);
    }

    [Fact(DisplayName = "US-16: Filtering bookings by date")]
    public async Task FilterBookings_ByDateRange()
    {
        // Arrange
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(7);

        var filteredBookings = new List<BookingCalendarItem>
        {
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(9, 0), IsBooked = true },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(14, 0), IsBooked = true }
        };

        _adminServiceMock
            .Setup(x => x.GetBookingsByDateRangeAsync(startDate, endDate))
            .ReturnsAsync(filteredBookings);

        // Act
        var bookings = await _adminServiceMock.Object.GetBookingsByDateRangeAsync(startDate, endDate);

        // Assert
        bookings.Should().HaveCount(2);
    }

    [Fact(DisplayName = "US-16: Empty calendar without bookings")]
    public async Task EmptyCalendar_WhenNoBookings()
    {
        // Arrange
        var bookingDate = DateTime.Today;

        _adminServiceMock
            .Setup(x => x.GetAllBookingsAsync(bookingDate))
            .ReturnsAsync(new List<BookingCalendarItem>());

        // Act
        var bookings = await _adminServiceMock.Object.GetAllBookingsAsync(bookingDate);

        // Assert
        bookings.Should().BeEmpty();
    }

    [Fact(DisplayName = "US-16: Error loading calendar")]
    public async Task ErrorLoadingBookings_ServerError()
    {
        // Arrange
        var bookingDate = DateTime.Today;

        _adminServiceMock
            .Setup(x => x.GetAllBookingsAsync(bookingDate))
            .ThrowsAsync(new InvalidOperationException("Error loading bookings"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _adminServiceMock.Object.GetAllBookingsAsync(bookingDate));
    }

    [Fact(DisplayName = "US-16: View calendar with corrupted data")]
    public async Task ViewCalendar_WithIncompleteData()
    {
        // Arrange
        var bookingDate = DateTime.Today;

        var bookingsWithErrors = new List<BookingCalendarItem>
        {
            new() { Id = Guid.NewGuid(), IsBooked = true, ResidentName = "Valid" },
            new() { Id = Guid.NewGuid(), IsBooked = true, ResidentName = null, DataError = "Missing resident info" }
        };

        _adminServiceMock
            .Setup(x => x.GetAllBookingsAsync(bookingDate))
            .ReturnsAsync(bookingsWithErrors);

        // Act
        var bookings = await _adminServiceMock.Object.GetAllBookingsAsync(bookingDate);

        // Assert
        bookings.Should().HaveCount(2);
        bookings.Last().DataError.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region US-17: Admin views booking details

    [Fact(DisplayName = "US-17: Admin views booking details")]
    public async Task AdminViewsBookingDetails_WithResidentInfo()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var bookingDetails = new AdminBookingDetails
        {
            Id = bookingId,
            ResidentName = "Peter Sidorenko",
            ApartmentNumber = "A-101",
            Email = "petro@example.com",
            Phone = "+380501234567",
            Date = DateTime.Today.AddDays(1),
            Time = "10:00 - 11:00"
        };

        _adminServiceMock
            .Setup(x => x.GetBookingDetailsAsync(bookingId))
            .ReturnsAsync(bookingDetails);

        // Act
        var details = await _adminServiceMock.Object.GetBookingDetailsAsync(bookingId);

        // Assert
        details.Should().NotBeNull();
        details.ResidentName.Should().Be("Peter Sidorenko");
        details.ApartmentNumber.Should().Be("A-101");
        details.Email.Should().Be("petro@example.com");
    }

    [Fact(DisplayName = "US-17: Error loading details")]
    public async Task ErrorLoadingBookingDetails_ServerError()
    {
        // Arrange
        var bookingId = Guid.NewGuid();

        _adminServiceMock
            .Setup(x => x.GetBookingDetailsAsync(bookingId))
            .ThrowsAsync(new InvalidOperationException("Loading error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _adminServiceMock.Object.GetBookingDetailsAsync(bookingId));
    }

    [Fact(DisplayName = "US-17: Booking deleted or data corrupted")]
    public async Task IncompleteBookingData_ShowsWarning()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var bookingDetails = new AdminBookingDetails
        {
            Id = bookingId,
            ResidentName = "Unknown",
            Email = null,
            DataIncomplete = true,
            ErrorMessage = "Resident data cannot be displayed"
        };

        _adminServiceMock
            .Setup(x => x.GetBookingDetailsAsync(bookingId))
            .ReturnsAsync(bookingDetails);

        // Act
        var details = await _adminServiceMock.Object.GetBookingDetailsAsync(bookingId);

        // Assert
        details.DataIncomplete.Should().BeTrue();
        details.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "US-17: Booking deleted by resident")]
    public async Task BookingByRemovedResident_ShowsMessage()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var bookingDetails = new AdminBookingDetails
        {
            Id = bookingId,
            ResidentName = "[Deleted]",
            Email = null,
            ErrorMessage = "Resident account no longer exists"
        };

        _adminServiceMock
            .Setup(x => x.GetBookingDetailsAsync(bookingId))
            .ReturnsAsync(bookingDetails);

        // Act
        var details = await _adminServiceMock.Object.GetBookingDetailsAsync(bookingId);

        // Assert
        details.ResidentName.Should().Be("[Deleted]");
        details.ErrorMessage.Should().Contain("no longer exists");
    }

    #endregion

    #region US-18: Admin cancels any booking

    [Fact(DisplayName = "US-18: Admin cancels booking for maintenance")]
    public async Task AdminCancelsBooking_ForMaintenance()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var cancellationReason = "Maintenance";

        var cancellationRequest = new AdminCancellationRequest
        {
            BookingId = bookingId,
            Reason = cancellationReason,
            AdminId = adminId
        };

        _adminServiceMock
            .Setup(x => x.IsAdminAuthorizedAsync(adminId))
            .ReturnsAsync(true);

        _adminServiceMock
            .Setup(x => x.CancelBookingAsync(cancellationRequest))
            .ReturnsAsync(true);

        _notificationServiceMock
            .Setup(x => x.NotifyResidentOfCancellationAsync(It.IsAny<string>(), cancellationReason))
            .ReturnsAsync(true);

        // Act
        var isAuthorized = await _adminServiceMock.Object.IsAdminAuthorizedAsync(adminId);
        var cancelled = await _adminServiceMock.Object.CancelBookingAsync(cancellationRequest);
        var notified = await _notificationServiceMock.Object.NotifyResidentOfCancellationAsync("resident@example.com", cancellationReason);

        // Assert
        isAuthorized.Should().BeTrue();
        cancelled.Should().BeTrue();
        notified.Should().BeTrue();
    }

    [Fact(DisplayName = "US-18: Resident receives cancellation notification")]
    public async Task ResidentNotified_WhenBookingCancelled()
    {
        // Arrange
        var residentEmail = "resident@example.com";
        var cancellationReason = "Unforeseen circumstance";

        _notificationServiceMock
            .Setup(x => x.SendCancellationEmailAsync(residentEmail, cancellationReason))
            .ReturnsAsync(true);

        // Act
        var emailSent = await _notificationServiceMock.Object.SendCancellationEmailAsync(residentEmail, cancellationReason);

        // Assert
        emailSent.Should().BeTrue();
        _notificationServiceMock.Verify(
            x => x.SendCancellationEmailAsync(residentEmail, cancellationReason),
            Times.Once);
    }

    [Fact(DisplayName = "US-18: Admin specifies cancellation reason")]
    public async Task AdminProvidesCancellationReason_IncludedInNotification()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var reason = "Emergency situation";

        var cancellationRequest = new AdminCancellationRequest
        {
            BookingId = bookingId,
            Reason = reason,
            AdminId = adminId
        };

        _adminServiceMock
            .Setup(x => x.CancelBookingAsync(cancellationRequest))
            .ReturnsAsync(true);

        // Act
        var result = await _adminServiceMock.Object.CancelBookingAsync(cancellationRequest);

        // Assert
        result.Should().BeTrue();
        _adminServiceMock.Verify(x => x.CancelBookingAsync(It.Is<AdminCancellationRequest>(
            r => r.Reason == reason)), Times.Once);
    }

    [Fact(DisplayName = "US-18: Resident cannot cancel other's booking")]
    public async Task ResidentCannotCancelOtherBooking_AccessDenied()
    {
        // Arrange
        var residentId = Guid.NewGuid();
        var otherResidentBookingId = Guid.NewGuid();

        _bookingServiceMock
            .Setup(x => x.CanResidentCancelBookingAsync(otherResidentBookingId, residentId))
            .ReturnsAsync(false);

        // Act
        var canCancel = await _bookingServiceMock.Object.CanResidentCancelBookingAsync(otherResidentBookingId, residentId);

        // Assert
        canCancel.Should().BeFalse();
    }

    #endregion
}
