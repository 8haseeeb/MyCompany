namespace SSO.Application.Auth;

public static class AuthEmailNormalizer
{
    public static string Normalize(string? email)
        => (email ?? string.Empty).Trim().ToLowerInvariant();
}
