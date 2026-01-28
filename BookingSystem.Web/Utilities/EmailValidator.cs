namespace BookingSystem.Web.Utilities;

public class EmailValidator
{
    public bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public string GenerateActivationToken()
    {
        return Guid.NewGuid().ToString("N");
    }

    public string Generate2FACode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}
