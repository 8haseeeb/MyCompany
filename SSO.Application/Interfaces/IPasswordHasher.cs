using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSO.Domain.Users;


namespace SSO.Application.Interfaces
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string hash, string password);
        /// <summary>False for null, empty, or values too short to be a valid BCrypt hash (avoids noisy verify attempts).</summary>
        bool LooksLikeBcryptHash(string? hash);
    }
}
