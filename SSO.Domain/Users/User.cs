using SSO.Domain.RefreshTokens;

namespace SSO.Domain.Users
{
    public class User
    {
        public int Id { get; private set; }
        public string UserName { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;

        public string Role { get; private set; } = "User"; // Default Role
        
        public string? CurrentSessionId { get; private set; }

        public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

        // EF Core requirement
        private User() { }

        public User(string userName, string email, string passwordHash, string role = "User")
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("UserName is required.");
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.");
            if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("Password hash is required.");

            UserName = userName;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
        }

        public void UpdateSession(string? newSessionId)
        {
            // Logic for session update
            CurrentSessionId = newSessionId;
        }

        public void UpdateRole(string newRole)
        {
            if (string.IsNullOrWhiteSpace(newRole)) throw new ArgumentException("Role cannot be empty.");
            Role = newRole;
        }

        public void UpdatePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash)) throw new ArgumentException("Password hash cannot be empty.");
            PasswordHash = newPasswordHash;
        }
    }
}
