using System.Net.Mail;

namespace Application.Auth;

public static class AuthValidator
{
    public const int MinPasswordLength = 8;
    public const int MaxPasswordLength = 128;

    public static string NormalizeEmail(string? email) => (email ?? string.Empty).Trim().ToLowerInvariant();

    public static bool IsValidEmail(string email)
        => email.Length is > 0 and <= 256
            && MailAddress.TryCreate(email, out var parsed)
            && parsed.Address == email
            && email.Contains('.', StringComparison.Ordinal);

    public static string? ValidatePassword(string? password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < MinPasswordLength)
        {
            return $"La contraseña debe tener al menos {MinPasswordLength} caracteres.";
        }

        if (password.Length > MaxPasswordLength)
        {
            return $"La contraseña no puede superar {MaxPasswordLength} caracteres.";
        }

        if (!password.Any(char.IsLetter) || !password.Any(char.IsDigit))
        {
            return "La contraseña debe incluir letras y números.";
        }

        return null;
    }
}
