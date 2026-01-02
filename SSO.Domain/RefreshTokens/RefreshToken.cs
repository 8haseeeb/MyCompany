using SSO.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSO.Domain.RefreshTokens
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public int UserId { get; set; }      // foreign key
        public User User { get; set; } = null!; // navigation property
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
    }

}
