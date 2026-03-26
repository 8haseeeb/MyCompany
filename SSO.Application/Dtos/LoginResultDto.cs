using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSO.Application.Dtos
{
    public class LoginResultDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public string UserName { get; set; } = null!;
        /// <summary>RBAC role from database (same value embedded in access token). Helps clients that decode JWT claims inconsistently.</summary>
        public string Role { get; set; } = null!;
        /// <summary>Set on registration responses when returning the same shape as login.</summary>
        public string? Message { get; set; }
    }
}
