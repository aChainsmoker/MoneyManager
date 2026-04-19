namespace MoneyManager.Utility.Encryption;

public static class PasswordHasher
{
    public static string GenerateSalt()
    {
        return BCrypt.Net.BCrypt.GenerateSalt();
    }
    
    public static string HashPassword(string password, string salt)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, salt);    
    }
}