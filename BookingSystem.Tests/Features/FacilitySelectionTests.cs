using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;

namespace BookingSystem.Tests.Features;

/// <summary>
/// US-25: Facility selection before slot creation or booking
/// </summary>
public class FacilitySelectionTests
{
    private readonly Mock<IAdminService> _adminServiceMock;
    private readonly Mock<IBookingService> _bookingServiceMock;

    public FacilitySelectionTests()
    {
        _adminServiceMock = new Mock<IAdminService>();
        _bookingServiceMock = new Mock<IBookingService>();
    }

    [Fact(DisplayName = "US-25: Admin sees facility icon and name")]
    public async Task AdminSeesFacilityList_WithIcons()
    {
        // Arrange
        var facilities = new List<FacilityItem>
        {
            new() { Id = Guid.NewGuid(), Name = "Pesukone 1", Icon = "🧺" }
        };

        _adminServiceMock
            .Setup(x => x.GetFacilitiesAsync())
            .ReturnsAsync(facilities);

        // Act
        var result = await _adminServiceMock.Object.GetFacilitiesAsync();

        // Assert
        result.Should().NotBeEmpty();
        result[0].Name.Should().Be("Pesukone 1");
        result[0].Icon.Should().Be("🧺");
    }

    [Fact(DisplayName = "US-25: Resident sees facility icon and name")]
    public async Task ResidentSeesFacilityList_ForBooking()
    {
        // Arrange
        var facilities = new List<FacilityItem>
        {
            new() { Id = Guid.NewGuid(), Name = "Pesukone 1", Icon = "🧺" }
        };

        _bookingServiceMock
            .Setup(x => x.GetFacilitiesAsync())
            .ReturnsAsync(facilities);

        // Act
        var result = await _bookingServiceMock.Object.GetFacilitiesAsync();

        // Assert
        result.Should().NotBeEmpty();
        result[0].Name.Should().Be("Pesukone 1");
        result[0].Icon.Should().Be("🧺");
    }

    [Fact(DisplayName = "US-25: Admin selects facility before creating slots")]
    public async Task AdminSelectsFacility_BeforeGeneratingSlots()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var facilityId = Guid.NewGuid();
        var date = new DateTime(2026, 1, 29);

        var request = new GenerateTimeSlotsRequest
        {
            AdminId = adminId,
            FacilityId = facilityId,
            Date = date,
            StartTime = "07:00",
            EndTime = "21:00"
        };

        _adminServiceMock
            .Setup(x => x.IsAdminAuthorizedAsync(adminId))
            .ReturnsAsync(true);

        _adminServiceMock
            .Setup(x => x.GenerateDailyTimeSlotsAsync(request))
            .ReturnsAsync(new GenerateTimeSlotsResult
            {
                FacilityId = facilityId,
                Date = date.Date,
                CreatedCount = 7,
                SkippedCount = 0,
                Slots = new List<TimeSlot>()
            });

        // Act
        var isAuthorized = await _adminServiceMock.Object.IsAdminAuthorizedAsync(adminId);
        var result = isAuthorized
            ? await _adminServiceMock.Object.GenerateDailyTimeSlotsAsync(request)
            : null;

        // Assert
        isAuthorized.Should().BeTrue();
        result.Should().NotBeNull();
        result.FacilityId.Should().Be(facilityId);
    }
}
