using SSO.Application.Interfaces;

namespace SSO.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
            => BCrypt.Net.BCrypt.HashPassword(password);

        public bool Verify(string hash, string password)
        {
            if (string.IsNullOrWhiteSpace(hash) || string.IsNullOrEmpty(password))
                return false;
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                // Malformed hash (not bcrypt) — treat as failed verification, not 500
                return false;
            }
        }

        public bool LooksLikeBcryptHash(string? hash)
            => !string.IsNullOrWhiteSpace(hash) && hash.Length >= 59;
    }
}
