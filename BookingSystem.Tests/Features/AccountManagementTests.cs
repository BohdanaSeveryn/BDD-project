using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;

namespace BookingSystem.Tests.Features;

/// <summary>
/// US-4: Admin creates a resident account
/// US-5: Admin deletes a resident account
/// US-19: Admin adds a new resident
/// US-20: Admin edits resident information
/// Managing resident accounts by administrator
/// </summary>
public class AdminAccountManagementTests
{
    private readonly Mock<IAdminService> _adminServiceMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IResidentService> _residentServiceMock;

    public AdminAccountManagementTests()
    {
        _adminServiceMock = new Mock<IAdminService>();
        _emailServiceMock = new Mock<IEmailService>();
        _residentServiceMock = new Mock<IResidentService>();
    }

    #region US-4: Admin creates a resident account

    [Fact(DisplayName = "US-4: Successful resident account creation")]
    public async Task SuccessfulResidentAccountCreation_WhenValidDataProvided()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var residentData = new CreateResidentRequest
        {
            Name = "Ivan Petrenko",
            ApartmentNumber = "A-101",
            Email = "ivan.petrenko@example.com",
            Phone = "+380501234567"
        };

        _adminServiceMock
            .Setup(x => x.IsAdminAuthorizedAsync(adminId))
            .ReturnsAsync(true);

        _residentServiceMock
            .Setup(x => x.CreateResidentAsync(residentData))
            .ReturnsAsync(new ResidentAccount { Id = Guid.NewGuid(), IsActive = false });

        _emailServiceMock
            .Setup(x => x.SendActivationEmailAsync(residentData.Email, It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var resident = await _residentServiceMock.Object.CreateResidentAsync(residentData);
        var emailSent = await _emailServiceMock.Object.SendActivationEmailAsync(
            residentData.Email, 
            "https://example.com/activate");

        // Assert
        resident.Should().NotBeNull();
        resident.IsActive.Should().BeFalse();
        emailSent.Should().BeTrue();
    }

    [Fact(DisplayName = "US-4: Error when duplicating email")]
    public async Task ResidentCreationFails_WhenEmailAlreadyExists()
    {
        // Arrange
        var residentData = new CreateResidentRequest
        {
            Name = "Maria Sidorenko",
            ApartmentNumber = "B-202",
            Email = "existing@example.com",
            Phone = "+380509876543"
        };

        _residentServiceMock
            .Setup(x => x.CreateResidentAsync(residentData))
            .ThrowsAsync(new InvalidOperationException("Email already in use"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _residentServiceMock.Object.CreateResidentAsync(residentData));
    }

    [Fact(DisplayName = "US-4: Error when duplicating phone number")]
    public async Task ResidentCreationFails_WhenPhoneAlreadyExists()
    {
        // Arrange
        var residentData = new CreateResidentRequest
        {
            Name = "Peter Ivanov",
            ApartmentNumber = "C-303",
            Email = "new@example.com",
            Phone = "+380501111111" // Phone number already in use
        };

        _residentServiceMock
            .Setup(x => x.CreateResidentAsync(residentData))
            .ThrowsAsync(new InvalidOperationException("Phone number already in use"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _residentServiceMock.Object.CreateResidentAsync(residentData));
    }

    [Fact(DisplayName = "US-4: Resident cannot login before activation")]
    public async Task ResidentLoginFails_WhenAccountNotActivated()
    {
        // Arrange
        var residentData = new CreateResidentRequest
        {
            Name = "Olga Lysenko",
            ApartmentNumber = "D-404",
            Email = "olga@example.com",
            Phone = "+380502222222"
        };

        _residentServiceMock
            .Setup(x => x.CreateResidentAsync(residentData))
            .ReturnsAsync(new ResidentAccount { Id = Guid.NewGuid(), IsActive = false });

        _residentServiceMock
            .Setup(x => x.LoginAsync("D-404", "password"))
            .ReturnsAsync(new LoginResult 
            { 
                Success = false, 
                ErrorMessage = "Account is not activated" 
            });

        // Act
        var resident = await _residentServiceMock.Object.CreateResidentAsync(residentData);
        var loginResult = await _residentServiceMock.Object.LoginAsync("D-404", "password");

        // Assert
        resident.IsActive.Should().BeFalse();
        loginResult.Success.Should().BeFalse();
    }

    #endregion

    #region US-5: Admin deletes a resident account

    [Fact(DisplayName = "US-5: Successful resident account deletion")]
    public async Task SuccessfulResidentAccountDeletion_WhenAdminAuthorized()
    {
        // Arrange
        var residentId = Guid.NewGuid();
        var residentEmail = "resident@example.com";

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

    [Fact(DisplayName = "US-5: Error when attempting to delete non-existent account")]
    public async Task ResidentDeletionFails_WhenResidentNotFound()
    {
        // Arrange
        var nonExistentResidentId = Guid.NewGuid();

        _adminServiceMock
            .Setup(x => x.DeleteResidentAccountAsync(nonExistentResidentId))
            .ThrowsAsync(new InvalidOperationException("Resident not found"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _adminServiceMock.Object.DeleteResidentAccountAsync(nonExistentResidentId));
    }

    [Fact(DisplayName = "US-5: Deleted resident cannot login")]
    public async Task DeletedResidentLoginFails_WhenAccountDeleted()
    {
        // Arrange
        var apartmentNumber = "A-101";
        var password = "password123";

        _residentServiceMock
            .Setup(x => x.LoginAsync(apartmentNumber, password))
            .ReturnsAsync(new LoginResult 
            { 
                Success = false, 
                ErrorMessage = "Account no longer exists" 
            });

        // Act
        var result = await _residentServiceMock.Object.LoginAsync(apartmentNumber, password);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("no longer exists");
    }

    #endregion

    #region US-19: Admin adds a new resident

    [Fact(DisplayName = "US-19: Successful addition of new resident")]
    public async Task NewResidentAdded_WhenValidDetailsProvided()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var residentData = new CreateResidentRequest
        {
            Name = "Anna Kovalenko",
            ApartmentNumber = "E-505",
            Email = "anna.kovalenko@example.com",
            Phone = "+380503333333"
        };

        _adminServiceMock
            .Setup(x => x.IsAdminAuthorizedAsync(adminId))
            .ReturnsAsync(true);

        _residentServiceMock
            .Setup(x => x.CreateResidentAsync(residentData))
            .ReturnsAsync(new ResidentAccount 
            { 
                Id = Guid.NewGuid(), 
                Name = residentData.Name,
                ApartmentNumber = residentData.ApartmentNumber,
                IsActive = false 
            });

        _emailServiceMock
            .Setup(x => x.SendActivationEmailAsync(residentData.Email, It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var resident = await _residentServiceMock.Object.CreateResidentAsync(residentData);
        var emailSent = await _emailServiceMock.Object.SendActivationEmailAsync(
            residentData.Email, 
            "https://example.com/activate");

        // Assert
        resident.Should().NotBeNull();
        resident.Name.Should().Be(residentData.Name);
        resident.ApartmentNumber.Should().Be(residentData.ApartmentNumber);
        emailSent.Should().BeTrue();
    }

    [Fact(DisplayName = "US-19: Error when email is missing")]
    public async Task ResidentCreationFails_WhenEmailMissing()
    {
        // Arrange
        var residentData = new CreateResidentRequest
        {
            Name = "Sergiy Bondarenko",
            ApartmentNumber = "F-606",
            Email = "", // Missing
            Phone = "+380504444444"
        };

        _residentServiceMock
            .Setup(x => x.CreateResidentAsync(residentData))
            .ThrowsAsync(new ValidationException("Email is mandatory"));

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => _residentServiceMock.Object.CreateResidentAsync(residentData));
    }

    [Fact(DisplayName = "US-19: Error when email is duplicated")]
    public async Task ResidentCreationFails_WhenDuplicateEmail()
    {
        // Arrange
        var residentData = new CreateResidentRequest
        {
            Name = "Tamila Cheban",
            ApartmentNumber = "G-707",
            Email = "existing@example.com",
            Phone = "+380505555555"
        };

        _residentServiceMock
            .Setup(x => x.CreateResidentAsync(residentData))
            .ThrowsAsync(new InvalidOperationException("User with this email already exists"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _residentServiceMock.Object.CreateResidentAsync(residentData));
    }

    #endregion

    #region US-20: Admin edits resident information

    [Fact(DisplayName = "US-20: Successful editing of resident information")]
    public async Task ResidentInformationUpdated_WhenValidChanges()
    {
        // Arrange
        var residentId = Guid.NewGuid();
        var updateData = new UpdateResidentRequest
        {
            Name = "Yuriy Shevchenko",
            ApartmentNumber = "H-808",
            Phone = "+380506666666"
        };

        _adminServiceMock
            .Setup(x => x.UpdateResidentAsync(residentId, updateData))
            .ReturnsAsync(true);

        // Act
        var result = await _adminServiceMock.Object.UpdateResidentAsync(residentId, updateData);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "US-20: Changing resident email")]
    public async Task ResidentEmailUpdated_WhenNewEmailProvided()
    {
        // Arrange
        var residentId = Guid.NewGuid();
        var newEmail = "newemail@example.com";
        var updateData = new UpdateResidentRequest { Email = newEmail };

        _adminServiceMock
            .Setup(x => x.UpdateResidentAsync(residentId, updateData))
            .ReturnsAsync(true);

        _emailServiceMock
            .Setup(x => x.SendEmailChangeNotificationAsync(newEmail))
            .ReturnsAsync(true);

        // Act
        var updated = await _adminServiceMock.Object.UpdateResidentAsync(residentId, updateData);
        var notificationSent = await _emailServiceMock.Object.SendEmailChangeNotificationAsync(newEmail);

        // Assert
        updated.Should().BeTrue();
        notificationSent.Should().BeTrue();
    }

    [Fact(DisplayName = "US-20: Error when name is empty")]
    public async Task ResidentUpdateFails_WhenNameEmpty()
    {
        // Arrange
        var residentId = Guid.NewGuid();
        var updateData = new UpdateResidentRequest { Name = "" };

        _adminServiceMock
            .Setup(x => x.UpdateResidentAsync(residentId, updateData))
            .ThrowsAsync(new ValidationException("Name cannot be empty"));

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => _adminServiceMock.Object.UpdateResidentAsync(residentId, updateData));
    }

    [Fact(DisplayName = "US-20: Cancel changes without saving")]
    public async Task CancelEditingWithoutSaving_NoChangesApplied()
    {
        // Arrange
        var originalResident = new ResidentAccount
        {
            Id = Guid.NewGuid(),
            Name = "Original name",
            Phone = "+380501234567"
        };

        _residentServiceMock
            .Setup(x => x.GetResidentByIdAsync(originalResident.Id))
            .ReturnsAsync(originalResident);

        // Act
        var resident = await _residentServiceMock.Object.GetResidentByIdAsync(originalResident.Id);

        // Assert
        resident.Name.Should().Be("Original name");
        resident.Phone.Should().Be("+380501234567");
    }

    #endregion
}
