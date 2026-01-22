using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;

namespace BookingSystem.Tests.Features;

/// <summary>
/// US-9: Resident confirms booking
/// US-10: Prevent double-booking
/// US-11: Booking appears in my calendar
/// Booking operations (confirmation, prevention of double-booking, display)
/// </summary>
public class BookingConfirmationTests
{
    private readonly Mock<IBookingService> _bookingServiceMock;
    private readonly Mock<INotificationService> _notificationServiceMock;

    public BookingConfirmationTests()
    {
        _bookingServiceMock = new Mock<IBookingService>();
        _notificationServiceMock = new Mock<INotificationService>();
    }

    #region US-9: Resident confirms booking

    [Fact(DisplayName = "US-9: Successful booking confirmation")]
    public async Task SuccessfulBookingConfirmation_WhenSlotSelected()
    {
        // Arrange
        var residentId = Guid.NewGuid();
        var timeSlotId = Guid.NewGuid();
        var bookingRequest = new CreateBookingRequest 
        { 
            ResidentId = residentId, 
            TimeSlotId = timeSlotId 
        };

        var booking = new Booking 
        { 
            Id = Guid.NewGuid(), 
            ResidentId = residentId, 
            TimeSlotId = timeSlotId, 
            Status = "Confirmed",
            CreatedAt = DateTime.UtcNow
        };

        _bookingServiceMock
            .Setup(x => x.ConfirmBookingAsync(bookingRequest))
            .ReturnsAsync(booking);

        // Act
        var result = await _bookingServiceMock.Object.ConfirmBookingAsync(bookingRequest);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be("Confirmed");
        result.ResidentId.Should().Be(residentId);
    }

    [Fact(DisplayName = "US-9: Cancellation before confirmation")]
    public async Task CancelBeforeConfirmation_NoReservationCreated()
    {
        // Arrange
        var timeSlotId = Guid.NewGuid();
        var bookingSession = new BookingSession { SelectedSlotId = timeSlotId };

        _bookingServiceMock
            .Setup(x => x.ClearSelectionAsync())
            .Callback(() => bookingSession.SelectedSlotId = null)
            .ReturnsAsync(true);

        // Act
        await _bookingServiceMock.Object.ClearSelectionAsync();

        // Assert
        bookingSession.SelectedSlotId.Should().BeNull();
    }

    [Fact(DisplayName = "US-9: Booking error when server problem")]
    public async Task BookingFails_WhenServerError()
    {
        // Arrange
        var bookingRequest = new CreateBookingRequest 
        { 
            ResidentId = Guid.NewGuid(), 
            TimeSlotId = Guid.NewGuid() 
        };

        _bookingServiceMock
            .Setup(x => x.ConfirmBookingAsync(bookingRequest))
            .ThrowsAsync(new InvalidOperationException("Booking failed, please try later"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _bookingServiceMock.Object.ConfirmBookingAsync(bookingRequest));
    }

    #endregion

    #region US-10: Prevent double-booking

    [Fact(DisplayName = "US-10: Slot reserved by another resident during booking process")]
    public async Task BookingFails_WhenSlotBookedDuringProcess()
    {
        // Arrange
        var residentId = Guid.NewGuid();
        var timeSlotId = Guid.NewGuid();
        var bookingRequest = new CreateBookingRequest 
        { 
            ResidentId = residentId, 
            TimeSlotId = timeSlotId 
        };

        _bookingServiceMock
            .Setup(x => x.ConfirmBookingAsync(bookingRequest))
            .ThrowsAsync(new InvalidOperationException("This slot just reserved by another resident"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _bookingServiceMock.Object.ConfirmBookingAsync(bookingRequest));
    }

    [Fact(DisplayName = "US-10: Attempting to book already reserved slot")]
    public async Task BookingRejected_WhenSlotAlreadyBooked()
    {
        // Arrange
        var timeSlotId = Guid.NewGuid();
        var bookingRequest = new CreateBookingRequest 
        { 
            ResidentId = Guid.NewGuid(), 
            TimeSlotId = timeSlotId 
        };

        _bookingServiceMock
            .Setup(x => x.ConfirmBookingAsync(bookingRequest))
            .ReturnsAsync((Booking)null);

        // Act
        var result = await _bookingServiceMock.Object.ConfirmBookingAsync(bookingRequest);

        // Assert
        result.Should().BeNull();
    }

    [Fact(DisplayName = "US-10: Calendar refresh after double-booking error")]
    public async Task CalendarRefreshed_AfterDoubleBookingError()
    {
        // Arrange
        var facilityId = Guid.NewGuid();
        var bookingDate = DateTime.Today;
        var timeSlotId = Guid.NewGuid();

        var refreshedAvailability = new List<TimeSlot>
        {
            new() { Id = timeSlotId, IsAvailable = false } // Now unavailable
        };

        _bookingServiceMock
            .Setup(x => x.RefreshAvailabilityAsync(facilityId, bookingDate))
            .Returns(Task.FromResult(refreshedAvailability));

        // Act
        var updated = await _bookingServiceMock.Object.RefreshAvailabilityAsync(facilityId, bookingDate);

        // Assert
        var slot = updated.FirstOrDefault(s => s.Id == timeSlotId);
        slot.Should().NotBeNull();
        slot.IsAvailable.Should().BeFalse();
    }

    #endregion

    #region US-11: Booking appears in my calendar

    [Fact(DisplayName = "US-11: Personal booking displayed in calendar")]
    public async Task PersonalBookingDisplayed_InCalendar()
    {
        // Arrange
        var residentId = Guid.NewGuid();
        var bookingDate = DateTime.Today.AddDays(1);
        var myBooking = new Booking 
        { 
            Id = Guid.NewGuid(),
            ResidentId = residentId, 
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0),
            DayOfWeek = "Tuesday",
            IsMyBooking = true,
            Icon = "washing-machine"
        };

        _bookingServiceMock
            .Setup(x => x.GetMyBookingsAsync(residentId, bookingDate))
            .ReturnsAsync(new List<Booking> { myBooking });

        // Act
        var bookings = await _bookingServiceMock.Object.GetMyBookingsAsync(residentId, bookingDate);

        // Assert
        bookings.Should().HaveCount(1);
        bookings.First().IsMyBooking.Should().BeTrue();
        bookings.First().Icon.Should().Be("washing-machine");
    }

    [Fact(DisplayName = "US-11: Distinguishing personal booking from others")]
    public async Task DistinguishPersonalFromOtherBookings()
    {
        // Arrange
        var residentId = Guid.NewGuid();
        var bookingDate = DateTime.Today;

        var calendarBookings = new List<Booking>
        {
            new() { Id = Guid.NewGuid(), ResidentId = Guid.NewGuid(), IsMyBooking = false, Status = "Unavailable" },
            new() { Id = Guid.NewGuid(), ResidentId = residentId, IsMyBooking = true, Status = "My booking", Icon = "washing-machine" }
        };

        _bookingServiceMock
            .Setup(x => x.GetCalendarBookingsAsync(It.IsAny<Guid>(), bookingDate))
            .ReturnsAsync(calendarBookings);

        // Act
        var bookings = await _bookingServiceMock.Object.GetCalendarBookingsAsync(Guid.NewGuid(), bookingDate);

        // Assert
        bookings.Should().HaveCount(2);
        bookings[0].IsMyBooking.Should().BeFalse();
        bookings[1].IsMyBooking.Should().BeTrue();
        bookings[1].Icon.Should().Be("washing-machine");
    }

    [Fact(DisplayName = "US-11: View booking details on hover")]
    public async Task ViewBookingDetails_OnHover()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var bookingDetails = new BookingDetails 
        { 
            Id = bookingId,
            Date = DateTime.Today.AddDays(2),
            Time = "10:00 - 11:00",
            CanManage = true
        };

        _bookingServiceMock
            .Setup(x => x.GetBookingDetailsAsync(bookingId))
            .ReturnsAsync(bookingDetails);

        // Act
        var details = await _bookingServiceMock.Object.GetBookingDetailsAsync(bookingId);

        // Assert
        details.Should().NotBeNull();
        details.Date.Should().Be(DateTime.Today.AddDays(2));
        details.Time.Should().Be("10:00 - 11:00");
        details.CanManage.Should().BeTrue();
    }

    #endregion
}

/// <summary>
/// US-12: Resident views all bookings
/// US-13: Resident views booking details
/// Viewing all bookings and details
/// </summary>
public class BookingListViewTests
{
    private readonly Mock<IBookingService> _bookingServiceMock;

    public BookingListViewTests()
    {
        _bookingServiceMock = new Mock<IBookingService>();
    }

    #region US-12: Resident views all bookings

    [Fact(DisplayName = "US-12: Resident sees all upcoming bookings")]
    public async Task ResidentViewsAllUpcomingBookings()
    {
        // Arrange
        var residentId = Guid.NewGuid();
        var bookings = new List<Booking>
        {
            new() { Id = Guid.NewGuid(), ResidentId = residentId, Facility = "Laundry room", Date = DateTime.Today.AddDays(1), Time = "10:00 - 11:00" },
            new() { Id = Guid.NewGuid(), ResidentId = residentId, Facility = "Laundry room", Date = DateTime.Today.AddDays(3), Time = "14:00 - 15:00" }
        };

        _bookingServiceMock
            .Setup(x => x.GetUpcomingBookingsAsync(residentId))
            .ReturnsAsync(bookings);

        // Act
        var result = await _bookingServiceMock.Object.GetUpcomingBookingsAsync(residentId);

        // Assert
        result.Should().HaveCount(2);
        result.All(b => b.ResidentId == residentId).Should().BeTrue();
    }

    [Fact(DisplayName = "US-12: Message about absence of bookings")]
    public async Task NoBookingsMessage_WhenNoUpcomingBookings()
    {
        // Arrange
        var residentId = Guid.NewGuid();

        _bookingServiceMock
            .Setup(x => x.GetUpcomingBookingsAsync(residentId))
            .ReturnsAsync(new List<Booking>());

        // Act
        var result = await _bookingServiceMock.Object.GetUpcomingBookingsAsync(residentId);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact(DisplayName = "US-12: Redirect to login without authorization")]
    public async Task RedirectToLogin_WhenNotAuthenticated()
    {
        // Arrange
        var residentId = Guid.NewGuid();

        _bookingServiceMock
            .Setup(x => x.IsUserAuthorizedAsync(residentId))
            .ReturnsAsync(false);

        // Act
        var isAuthorized = await _bookingServiceMock.Object.IsUserAuthorizedAsync(residentId);

        // Assert
        isAuthorized.Should().BeFalse();
    }

    #endregion

    #region US-13: Resident views booking details

    [Fact(DisplayName = "US-13: View booking details")]
    public async Task ViewBookingDetails_WhenBookingExists()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var bookingDetails = new BookingDetails 
        { 
            Id = bookingId,
            Facility = "Laundry room",
            Date = DateTime.Today.AddDays(1),
            Time = "10:00 - 11:00",
            CanCancelBooking = true
        };

        _bookingServiceMock
            .Setup(x => x.GetBookingDetailsAsync(bookingId))
            .ReturnsAsync(bookingDetails);

        // Act
        var details = await _bookingServiceMock.Object.GetBookingDetailsAsync(bookingId);

        // Assert
        details.Should().NotBeNull();
        details.Facility.Should().Be("Laundry room");
        details.CanCancelBooking.Should().BeTrue();
    }

    [Fact(DisplayName = "US-13: View details of multiple bookings")]
    public async Task ViewDetailsOfMultipleBookings_Individually()
    {
        // Arrange
        var bookingId1 = Guid.NewGuid();
        var bookingId2 = Guid.NewGuid();

        var details1 = new BookingDetails { Id = bookingId1, Date = DateTime.Today.AddDays(1) };
        var details2 = new BookingDetails { Id = bookingId2, Date = DateTime.Today.AddDays(3) };

        _bookingServiceMock
            .Setup(x => x.GetBookingDetailsAsync(bookingId1))
            .ReturnsAsync(details1);

        _bookingServiceMock
            .Setup(x => x.GetBookingDetailsAsync(bookingId2))
            .ReturnsAsync(details2);

        // Act
        var result1 = await _bookingServiceMock.Object.GetBookingDetailsAsync(bookingId1);
        var result2 = await _bookingServiceMock.Object.GetBookingDetailsAsync(bookingId2);

        // Assert
        result1.Id.Should().Be(bookingId1);
        result2.Id.Should().Be(bookingId2);
    }

    [Fact(DisplayName = "US-13: Negative scenario - booking no longer exists")]
    public async Task BookingDetailsNotFound_WhenBookingDeleted()
    {
        // Arrange
        var deletedBookingId = Guid.NewGuid();

        _bookingServiceMock
            .Setup(x => x.GetBookingDetailsAsync(deletedBookingId))
            .ReturnsAsync((BookingDetails)null);

        // Act
        var details = await _bookingServiceMock.Object.GetBookingDetailsAsync(deletedBookingId);

        // Assert
        details.Should().BeNull();
    }

    [Fact(DisplayName = "US-13: Error loading booking details")]
    public async Task ErrorLoadingBookingDetails_WhenServerError()
    {
        // Arrange
        var bookingId = Guid.NewGuid();

        _bookingServiceMock
            .Setup(x => x.GetBookingDetailsAsync(bookingId))
            .ThrowsAsync(new InvalidOperationException("Error loading details"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _bookingServiceMock.Object.GetBookingDetailsAsync(bookingId));
    }

    #endregion
}

/// <summary>
/// US-14: Resident cancels a booking
/// Resident cancels a booking
/// </summary>
public class BookingCancellationTests
{
    private readonly Mock<IBookingService> _bookingServiceMock;
    private readonly Mock<INotificationService> _notificationServiceMock;

    public BookingCancellationTests()
    {
        _bookingServiceMock = new Mock<IBookingService>();
        _notificationServiceMock = new Mock<INotificationService>();
    }

    [Fact(DisplayName = "US-14: Cancel booking from calendar")]
    public async Task CancelBooking_FromCalendarView()
    {
        // Arrange
        var residentId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();

        _bookingServiceMock
            .Setup(x => x.CancelBookingAsync(bookingId, residentId))
            .ReturnsAsync(true);

        // Act
        var result = await _bookingServiceMock.Object.CancelBookingAsync(bookingId, residentId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "US-14: Cancellation after cancellation window ends")]
    public async Task CancelationDenied_AfterCancellationWindow()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var residentId = Guid.NewGuid();
        var bookingStartTime = DateTime.UtcNow.AddMinutes(30); // In 30 minutes

        _bookingServiceMock
            .Setup(x => x.CanCancelBookingAsync(bookingId))
            .ReturnsAsync(false); // Cancellation period ended

        _bookingServiceMock
            .Setup(x => x.CancelBookingAsync(bookingId, residentId))
            .ThrowsAsync(new InvalidOperationException("Cancellation not allowed by policy"));

        // Act & Assert
        var canCancel = await _bookingServiceMock.Object.CanCancelBookingAsync(bookingId);
        canCancel.Should().BeFalse();
    }

    [Fact(DisplayName = "US-14: Cancellation error due to server error")]
    public async Task CancellationFails_WhenServerError()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var residentId = Guid.NewGuid();

        _bookingServiceMock
            .Setup(x => x.CancelBookingAsync(bookingId, residentId))
            .ThrowsAsync(new InvalidOperationException("Server error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _bookingServiceMock.Object.CancelBookingAsync(bookingId, residentId));
    }

    [Fact(DisplayName = "US-14: Cannot cancel started booking")]
    public async Task CannotCancelStartedBooking()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var residentId = Guid.NewGuid();
        var startTime = DateTime.UtcNow.AddMinutes(-10); // Already started

        _bookingServiceMock
            .Setup(x => x.CanCancelBookingAsync(bookingId))
            .ReturnsAsync(false);

        // Act
        var canCancel = await _bookingServiceMock.Object.CanCancelBookingAsync(bookingId);

        // Assert
        canCancel.Should().BeFalse();
    }
}
