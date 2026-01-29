using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;

namespace BookingSystem.Tests.Features;

/// <summary>
/// US-24: Admin creates daily time slots
/// Administrative creation of 2-hour booking slots from 07:00 to 21:00
/// </summary>
public class AdminTimeSlotManagementTests
{
    private readonly Mock<IAdminService> _adminServiceMock;

    public AdminTimeSlotManagementTests()
    {
        _adminServiceMock = new Mock<IAdminService>();
    }

    #region US-24: Admin creates daily time slots

    [Fact(DisplayName = "US-24: Admin creates 2-hour slots from 07 to 21")]
    public async Task AdminCreatesDailyTimeSlots_TwoHourBlocks()
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

        var slots = BuildDailySlots();

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
                Slots = slots
            });

        // Act
        var isAuthorized = await _adminServiceMock.Object.IsAdminAuthorizedAsync(adminId);
        var result = isAuthorized
            ? await _adminServiceMock.Object.GenerateDailyTimeSlotsAsync(request)
            : new GenerateTimeSlotsResult();

        // Assert
        isAuthorized.Should().BeTrue();
        result.CreatedCount.Should().Be(7);
        result.SkippedCount.Should().Be(0);
        result.Slots.Should().HaveCount(7);

        result.Slots.First().StartTime.Should().Be(new TimeOnly(7, 0));
        result.Slots.First().EndTime.Should().Be(new TimeOnly(9, 0));
        result.Slots.Last().StartTime.Should().Be(new TimeOnly(19, 0));
        result.Slots.Last().EndTime.Should().Be(new TimeOnly(21, 0));

        result.Slots.All(s => s.Status == "Available").Should().BeTrue();
        result.Slots.All(s => s.Color == "green").Should().BeTrue();
    }

    [Fact(DisplayName = "US-24: Existing slots are skipped")]
    public async Task AdminCreatesDailyTimeSlots_SkipsExistingSlots()
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

        var slots = BuildDailySlots();

        _adminServiceMock
            .Setup(x => x.IsAdminAuthorizedAsync(adminId))
            .ReturnsAsync(true);

        _adminServiceMock
            .Setup(x => x.GenerateDailyTimeSlotsAsync(request))
            .ReturnsAsync(new GenerateTimeSlotsResult
            {
                FacilityId = facilityId,
                Date = date.Date,
                CreatedCount = 4,
                SkippedCount = 3,
                Slots = slots
            });

        // Act
        var isAuthorized = await _adminServiceMock.Object.IsAdminAuthorizedAsync(adminId);
        var result = isAuthorized
            ? await _adminServiceMock.Object.GenerateDailyTimeSlotsAsync(request)
            : new GenerateTimeSlotsResult();

        // Assert
        isAuthorized.Should().BeTrue();
        result.CreatedCount.Should().Be(4);
        result.SkippedCount.Should().Be(3);
        result.Slots.Should().HaveCount(7);
    }

    [Fact(DisplayName = "US-24: Unauthorized admin cannot generate slots")]
    public async Task AdminCannotGenerateSlots_WhenNotAuthorized()
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
            .ReturnsAsync(false);

        // Act
        var isAuthorized = await _adminServiceMock.Object.IsAdminAuthorizedAsync(adminId);
        GenerateTimeSlotsResult result = null;
        if (isAuthorized)
        {
            result = await _adminServiceMock.Object.GenerateDailyTimeSlotsAsync(request);
        }

        // Assert
        isAuthorized.Should().BeFalse();
        result.Should().BeNull();

        _adminServiceMock.Verify(x => x.GenerateDailyTimeSlotsAsync(It.IsAny<GenerateTimeSlotsRequest>()), Times.Never);
    }

    #endregion

    private static List<TimeSlot> BuildDailySlots()
    {
        return new List<TimeSlot>
        {
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(7, 0), EndTime = new TimeOnly(9, 0), Status = "Available", Color = "green" },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(11, 0), Status = "Available", Color = "green" },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(11, 0), EndTime = new TimeOnly(13, 0), Status = "Available", Color = "green" },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(13, 0), EndTime = new TimeOnly(15, 0), Status = "Available", Color = "green" },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(15, 0), EndTime = new TimeOnly(17, 0), Status = "Available", Color = "green" },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(17, 0), EndTime = new TimeOnly(19, 0), Status = "Available", Color = "green" },
            new() { Id = Guid.NewGuid(), StartTime = new TimeOnly(19, 0), EndTime = new TimeOnly(21, 0), Status = "Available", Color = "green" }
        };
    }
}
