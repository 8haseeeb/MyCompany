using SSO.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSO.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user); 
    }
}
