namespace SSO.Application.Auth;

/// <summary>
/// Thrown when registration targets an email that already exists. Maps to HTTP 409 (not 500).
/// </summary>
public sealed class DuplicateUserException : Exception
{
    public DuplicateUserException()
        : base("An account with this email already exists.")
    {
    }
}
