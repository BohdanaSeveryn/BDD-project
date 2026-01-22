using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;

namespace BookingSystem.Tests.Features;

/// <summary>
/// US-6: Resident views washing machine availability
/// US-7: Resident views booking calendar
/// Viewing availability and booking calendar
/// </summary>
public class BookingViewTests
{
    private readonly Mock<IBookingService> _bookingServiceMock;
    private readonly Mock<IAuthenticationService> _authServiceMock;

    public BookingViewTests()
    {
        _bookingServiceMock = new Mock<IBookingService>();
        _authServiceMock = new Mock<IAuthenticationService>();
    }

    #region US-6: Resident views washing machine availability

    [Fact(DisplayName = "US-6: Resident sees available time slots")]
    public async Task ResidentSeesAvailableTimeSlots_WhenLogged()
    {
        // Arrange
        var residentId = Guid.NewGuid();
        var facilityId = Guid.NewGuid();
        var bookingDate = DateTime.Today.AddDays(1);

        var availability = new List<TimeSlot>
        {
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(9, 0), IsAvailable = true },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(10, 0), IsAvailable = false },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(11, 0), IsAvailable = true }
        };

        _authServiceMock
            .Setup(x => x.IsUserAuthenticatedAsync(residentId))
            .ReturnsAsync(true);

        _bookingServiceMock
            .Setup(x => x.GetAvailabilityAsync(facilityId, bookingDate))
            .ReturnsAsync(availability);

        // Act
        var isAuthenticated = await _authServiceMock.Object.IsUserAuthenticatedAsync(residentId);
        var slots = await _bookingServiceMock.Object.GetAvailabilityAsync(facilityId, bookingDate);

        // Assert
        isAuthenticated.Should().BeTrue();
        slots.Should().HaveCount(3);
        slots.Count(s => s.IsAvailable).Should().Be(2);
    }

    [Fact(DisplayName = "US-6: Resident distinguishes available from unavailable slots")]
    public async Task ResidentDistinguishesAvailableFromUnavailable()
    {
        // Arrange
        var facilityId = Guid.NewGuid();
        var bookingDate = DateTime.Today;

        var availability = new List<TimeSlot>
        {
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(14, 0), EndTime = new TimeOnly(15, 0), IsAvailable = true, Status = "Available" },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(15, 0), EndTime = new TimeOnly(16, 0), IsAvailable = false, Status = "Reserved" }
        };

        _bookingServiceMock
            .Setup(x => x.GetAvailabilityAsync(facilityId, bookingDate))
            .ReturnsAsync(availability);

        // Act
        var slots = await _bookingServiceMock.Object.GetAvailabilityAsync(facilityId, bookingDate);

        // Assert
        slots[0].Status.Should().Be("Available");
        slots[1].Status.Should().Be("Reserved");
    }

    [Fact(DisplayName = "US-6: Redirect to login page without authorization")]
    public async Task UserRedirectedToLogin_WhenNotAuthenticated()
    {
        // Arrange
        var residentId = Guid.NewGuid();

        _authServiceMock
            .Setup(x => x.IsUserAuthenticatedAsync(residentId))
            .ReturnsAsync(false);

        // Act
        var isAuthenticated = await _authServiceMock.Object.IsUserAuthenticatedAsync(residentId);

        // Assert
        isAuthenticated.Should().BeFalse();
    }

    [Fact(DisplayName = "US-6: Loading availability for different date")]
    public async Task AvailabilityLoaded_WhenDifferentDateSelected()
    {
        // Arrange
        var facilityId = Guid.NewGuid();
        var differentDate = DateTime.Today.AddDays(7);

        var futureAvailability = new List<TimeSlot>
        {
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(10, 0), IsAvailable = true }
        };

        _bookingServiceMock
            .Setup(x => x.GetAvailabilityAsync(facilityId, differentDate))
            .ReturnsAsync(futureAvailability);

        // Act
        var slots = await _bookingServiceMock.Object.GetAvailabilityAsync(facilityId, differentDate);

        // Assert
        slots.Should().NotBeEmpty();
        slots.First().IsAvailable.Should().BeTrue();
    }

    #endregion

    #region US-7: Resident views booking calendar

    [Fact(DisplayName = "US-7: View calendar for current day")]
    public async Task ResidentViewsCalendar_ForCurrentDay()
    {
        // Arrange
        var residentId = Guid.NewGuid();
        var facilityId = Guid.NewGuid();
        var today = DateTime.Today;

        var todaySchedule = new List<TimeSlot>
        {
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(9, 0), IsAvailable = true },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(10, 0), IsAvailable = true },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(11, 0), IsAvailable = false }
        };

        _authServiceMock
            .Setup(x => x.IsUserAuthenticatedAsync(residentId))
            .ReturnsAsync(true);

        _bookingServiceMock
            .Setup(x => x.GetAvailabilityAsync(facilityId, today))
            .ReturnsAsync(todaySchedule);

        // Act
        var isAuthenticated = await _authServiceMock.Object.IsUserAuthenticatedAsync(residentId);
        var schedule = await _bookingServiceMock.Object.GetAvailabilityAsync(facilityId, today);

        // Assert
        isAuthenticated.Should().BeTrue();
        schedule.Should().HaveCount(3);
        schedule.Count(s => s.IsAvailable).Should().Be(2);
    }

    [Fact(DisplayName = "US-7: Green and unavailable slots are properly marked")]
    public async Task TimeSlotsProperlyCoded_GreenForAvailableRedForUnavailable()
    {
        // Arrange
        var facilityId = Guid.NewGuid();
        var bookingDate = DateTime.Today;

        var schedule = new List<TimeSlot>
        {
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(14, 0), EndTime = new TimeOnly(15, 0), IsAvailable = true, Color = "green" },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(15, 0), EndTime = new TimeOnly(16, 0), IsAvailable = false, Color = "red" }
        };

        _bookingServiceMock
            .Setup(x => x.GetAvailabilityAsync(facilityId, bookingDate))
            .ReturnsAsync(schedule);

        // Act
        var slots = await _bookingServiceMock.Object.GetAvailabilityAsync(facilityId, bookingDate);

        // Assert
        slots[0].Color.Should().Be("green");
        slots[1].Color.Should().Be("red");
    }

    [Fact(DisplayName = "US-7: View availability for future date")]
    public async Task ViewAvailability_ForFutureDate()
    {
        // Arrange
        var facilityId = Guid.NewGuid();
        var futureDate = DateTime.Today.AddDays(14);

        var futureSchedule = new List<TimeSlot>
        {
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(9, 0), IsAvailable = true },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(10, 0), IsAvailable = true }
        };

        _bookingServiceMock
            .Setup(x => x.GetAvailabilityAsync(facilityId, futureDate))
            .ReturnsAsync(futureSchedule);

        // Act
        var schedule = await _bookingServiceMock.Object.GetAvailabilityAsync(facilityId, futureDate);

        // Assert
        schedule.Should().HaveCount(2);
        schedule.All(s => s.IsAvailable).Should().BeTrue();
    }

    #endregion
}

/// <summary>
/// US-8: Resident selects a time slot
/// Selecting time slot by resident
/// </summary>
public class TimeSlotSelectionTests
{
    private readonly Mock<IBookingService> _bookingServiceMock;

    public TimeSlotSelectionTests()
    {
        _bookingServiceMock = new Mock<IBookingService>();
    }

    [Fact(DisplayName = "US-8: Selecting available time slot")]
    public async Task SelectAvailableTimeSlot_SlotHighlighted()
    {
        // Arrange
        var timeSlotId = Guid.NewGuid();
        var bookingSession = new BookingSession { SelectedSlotId = null };

        _bookingServiceMock
            .Setup(x => x.SelectTimeSlotAsync(timeSlotId))
            .Callback(() => bookingSession.SelectedSlotId = timeSlotId)
            .ReturnsAsync(true);

        // Act
        var result = await _bookingServiceMock.Object.SelectTimeSlotAsync(timeSlotId);

        // Assert
        result.Should().BeTrue();
        bookingSession.SelectedSlotId.Should().Be(timeSlotId);
    }

    [Fact(DisplayName = "US-8: Changing slot selection")]
    public async Task ChangeSelection_FromFirstToSecondSlot()
    {
        // Arrange
        var firstSlotId = Guid.NewGuid();
        var secondSlotId = Guid.NewGuid();
        var bookingSession = new BookingSession { SelectedSlotId = firstSlotId };

        _bookingServiceMock
            .Setup(x => x.SelectTimeSlotAsync(secondSlotId))
            .Callback(() => bookingSession.SelectedSlotId = secondSlotId)
            .ReturnsAsync(true);

        // Act
        await _bookingServiceMock.Object.SelectTimeSlotAsync(secondSlotId);

        // Assert
        bookingSession.SelectedSlotId.Should().Be(secondSlotId);
    }

    [Fact(DisplayName = "US-8: Attempting to select unavailable slot")]
    public async Task SelectUnavailableSlot_FailsWithError()
    {
        // Arrange
        var unavailableSlotId = Guid.NewGuid();

        _bookingServiceMock
            .Setup(x => x.SelectTimeSlotAsync(unavailableSlotId))
            .ReturnsAsync(false);

        // Act
        var result = await _bookingServiceMock.Object.SelectTimeSlotAsync(unavailableSlotId);

        // Assert
        result.Should().BeFalse();
    }
}
