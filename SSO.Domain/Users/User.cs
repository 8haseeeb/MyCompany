using SSO.Domain.RefreshTokens;

namespace SSO.Domain.Users
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();


        public User(string userName, string email, string passwordHash)
        {
            UserName = userName;
            Email = email;
            PasswordHash = passwordHash;
        }
    }
}
