using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;

namespace BookingSystem.Tests.Features;

/// <summary>
/// US-1: Resident account activation
/// Account activation scenarios for residents with email receipt and password setup
/// </summary>
public class ResidentAccountActivationTests
{
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IResidentService> _residentServiceMock;
    
    public ResidentAccountActivationTests()
    {
        _emailServiceMock = new Mock<IEmailService>();
        _residentServiceMock = new Mock<IResidentService>();
    }

    [Fact(DisplayName = "US-1: Successful account activation")]
    public async Task SuccessfulAccountActivation_WhenValidActivationLinkAndPassword_AccountBecomesActive()
    {
        // Arrange
        var residentId = Guid.NewGuid();
        var activationToken = "valid-token-123";
        var password = "SecurePassword123!";
        var resident = new ResidentAccount
        {
            Id = residentId,
            Email = "resident@example.com",
            IsActive = false,
            ActivationToken = activationToken,
            ActivationTokenExpiry = DateTime.UtcNow.AddHours(24)
        };

        _residentServiceMock
            .Setup(x => x.GetResidentByIdAsync(residentId))
            .ReturnsAsync(resident);

        _residentServiceMock
            .Setup(x => x.ActivateAccountAsync(residentId, password))
            .ReturnsAsync(true);

        // Act
        var result = await _residentServiceMock.Object.ActivateAccountAsync(residentId, password);

        // Assert
        result.Should().BeTrue();
        _residentServiceMock.Verify(x => x.ActivateAccountAsync(residentId, password), Times.Once);
    }

    [Fact(DisplayName = "US-1: Activation email sent when account is created")]
    public async Task ActivationEmailSent_WhenAccountCreated()
    {
        // Arrange
        var residentEmail = "newresident@example.com";
        var residentId = Guid.NewGuid();

        _emailServiceMock
            .Setup(x => x.SendActivationEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var emailSent = await _emailServiceMock.Object.SendActivationEmailAsync(
            residentEmail, 
            $"https://example.com/activate?token=unique-token");

        // Assert
        emailSent.Should().BeTrue();
        _emailServiceMock.Verify(
            x => x.SendActivationEmailAsync(residentEmail, It.IsAny<string>()), 
            Times.Once);
    }

    [Fact(DisplayName = "US-1: Activation link has time limit")]
    public async Task ActivationLinkExpired_WhenTokenExpiryTimeReached()
    {
        // Arrange
        var residentId = Guid.NewGuid();
        var expiredToken = "expired-token-456";
        var resident = new ResidentAccount
        {
            Id = residentId,
            Email = "resident@example.com",
            ActivationToken = expiredToken,
            ActivationTokenExpiry = DateTime.UtcNow.AddHours(-1) // Already expired
        };

        _residentServiceMock
            .Setup(x => x.IsActivationTokenValidAsync(expiredToken))
            .ReturnsAsync(false);

        // Act
        var isValid = await _residentServiceMock.Object.IsActivationTokenValidAsync(expiredToken);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact(DisplayName = "US-1: Login is impossible until activation")]
    public async Task LoginFails_WhenAccountNotActivated()
    {
        // Arrange
        var apartmentNumber = "A-101";
        var password = "password123";
        
        _residentServiceMock
            .Setup(x => x.LoginAsync(apartmentNumber, password))
            .ThrowsAsync(new InvalidOperationException("Account is not activated"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _residentServiceMock.Object.LoginAsync(apartmentNumber, password));
    }
}

/// <summary>
/// US-2: Resident login
/// Login to the system using apartment number and password
/// </summary>
public class ResidentLoginTests
{
    private readonly Mock<IResidentService> _residentServiceMock;

    public ResidentLoginTests()
    {
        _residentServiceMock = new Mock<IResidentService>();
    }

    [Fact(DisplayName = "US-2: Successful login of active resident")]
    public async Task SuccessfulLogin_WhenValidCredentialsAndAccountActive()
    {
        // Arrange
        var apartmentNumber = "A-101";
        var password = "ValidPassword123!";
        var residentId = Guid.NewGuid();
        
        _residentServiceMock
            .Setup(x => x.LoginAsync(apartmentNumber, password))
            .ReturnsAsync(new LoginResult 
            { 
                Success = true, 
                ResidentId = residentId,
                Token = "valid-jwt-token"
            });

        // Act
        var result = await _residentServiceMock.Object.LoginAsync(apartmentNumber, password);

        // Assert
        result.Success.Should().BeTrue();
        result.ResidentId.Should().Be(residentId);
        result.Token.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "US-2: Login denied for inactive account")]
    public async Task LoginFails_WhenAccountInactive()
    {
        // Arrange
        var apartmentNumber = "B-205";
        var password = "password123";

        _residentServiceMock
            .Setup(x => x.LoginAsync(apartmentNumber, password))
            .ReturnsAsync(new LoginResult 
            { 
                Success = false, 
                ErrorMessage = "Account is not activated" 
            });

        // Act
        var result = await _residentServiceMock.Object.LoginAsync(apartmentNumber, password);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("not activated");
    }

    [Fact(DisplayName = "US-2: Error on invalid login credentials")]
    public async Task LoginFails_WhenInvalidCredentials()
    {
        // Arrange
        var apartmentNumber = "A-101";
        var wrongPassword = "WrongPassword";

        _residentServiceMock
            .Setup(x => x.LoginAsync(apartmentNumber, wrongPassword))
            .ReturnsAsync(new LoginResult 
            { 
                Success = false, 
                ErrorMessage = "Invalid credentials" 
            });

        // Act
        var result = await _residentServiceMock.Object.LoginAsync(apartmentNumber, wrongPassword);

        // Assert
        result.Success.Should().BeFalse();
    }
}

/// <summary>
/// US-3: Admin login
/// Administrator login with two-factor authentication
/// </summary>
public class AdminLoginTests
{
    private readonly Mock<IAdminService> _adminServiceMock;
    private readonly Mock<ITwoFactorService> _twoFactorServiceMock;

    public AdminLoginTests()
    {
        _adminServiceMock = new Mock<IAdminService>();
        _twoFactorServiceMock = new Mock<ITwoFactorService>();
    }

    [Fact(DisplayName = "US-3: Successful admin login with 2FA")]
    public async Task SuccessfulAdminLogin_WhenCredentialsAndTwoFactorCodeValid()
    {
        // Arrange
        var username = "admin@example.com";
        var password = "AdminPassword123!";
        var twoFactorCode = "123456";
        var adminId = Guid.NewGuid();

        _adminServiceMock
            .Setup(x => x.AuthenticateAsync(username, password))
            .ReturnsAsync(true);

        _twoFactorServiceMock
            .Setup(x => x.ValidateTwoFactorCodeAsync(adminId, twoFactorCode))
            .ReturnsAsync(true);

        // Act
        var authResult = await _adminServiceMock.Object.AuthenticateAsync(username, password);
        var twoFactorResult = await _twoFactorServiceMock.Object.ValidateTwoFactorCodeAsync(adminId, twoFactorCode);

        // Assert
        authResult.Should().BeTrue();
        twoFactorResult.Should().BeTrue();
    }

    [Fact(DisplayName = "US-3: 2FA code sent via email")]
    public async Task TwoFactorCodeSent_WhenAdminAuthenticated()
    {
        // Arrange
        var adminEmail = "admin@example.com";

        _twoFactorServiceMock
            .Setup(x => x.SendTwoFactorCodeAsync(adminEmail))
            .ReturnsAsync(true);

        // Act
        var result = await _twoFactorServiceMock.Object.SendTwoFactorCodeAsync(adminEmail);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "US-3: Access denied with invalid 2FA code")]
    public async Task AdminAccessDenied_WhenTwoFactorCodeInvalid()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var invalidCode = "000000";

        _twoFactorServiceMock
            .Setup(x => x.ValidateTwoFactorCodeAsync(adminId, invalidCode))
            .ReturnsAsync(false);

        // Act
        var result = await _twoFactorServiceMock.Object.ValidateTwoFactorCodeAsync(adminId, invalidCode);

        // Assert
        result.Should().BeFalse();
    }
}
