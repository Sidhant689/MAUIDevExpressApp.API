using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.Shared.DTOs
{
    public class RefreshTokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
        public string Username { get; set; }
        public List<RoleDTO>? Roles { get; set; }
        public List<PermissionDTO>? Permissions { get; set; }
    }
}
