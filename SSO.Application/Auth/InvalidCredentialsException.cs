namespace SSO.Application.Auth;

/// <summary>
/// Thrown when email/password do not match. Maps to HTTP 401 (not 500).
/// </summary>
public sealed class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException()
        : base("Invalid credentials")
    {
    }
}
