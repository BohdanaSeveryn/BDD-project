namespace BookingSystem.Web.DTOs;

public class LoginRequest
{
    public string ApartmentNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public Guid ResidentId { get; set; }
    public string ApartmentNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class AdminLoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AdminLoginResponse
{
    public string Token { get; set; } = string.Empty;
    public Guid AdminId { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool RequiresTwoFactor { get; set; }
}

public class TwoFactorRequest
{
    public string Code { get; set; } = string.Empty;
}

public class TwoFactorResponse
{
    public string Token { get; set; } = string.Empty;
    public Guid AdminId { get; set; }
}

public class ActivationRequest
{
    public string Password { get; set; } = string.Empty;
}

public class ResidentAccount
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ApartmentNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CreateResidentRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ApartmentNumber { get; set; } = string.Empty;
}

public class UpdateResidentRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ApartmentNumber { get; set; } = string.Empty;
}

public class CreateBookingRequest
{
    public Guid ResidentId { get; set; }
    public Guid TimeSlotId { get; set; }
    public Guid FacilityId { get; set; }
}

public class BookingResponse
{
    public Guid Id { get; set; }
    public DateTime BookingDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string FacilityName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class BookingDetails
{
    public Guid Id { get; set; }
    public DateTime BookingDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string FacilityName { get; set; } = string.Empty;
    public string ResidentName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class AdminBookingDetails
{
    public Guid Id { get; set; }
    public DateTime BookingDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string FacilityName { get; set; } = string.Empty;
    public string ResidentName { get; set; } = string.Empty;
    public string ResidentApartment { get; set; } = string.Empty;
    public string ResidentEmail { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class TimeSlotResponse
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public class AdminCancellationRequest
{
    public Guid BookingId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class BookingConfirmationEmail
{
    public string ResidentEmail { get; set; } = string.Empty;
    public string ResidentName { get; set; } = string.Empty;
    public string FacilityName { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}

public class ErrorResponse
{
    public string ErrorMessage { get; set; } = string.Empty;
    public int StatusCode { get; set; }
}
