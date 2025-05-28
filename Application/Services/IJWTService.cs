using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IJWTService
    {
        string GenerateSecurityToken(string id, string email, string firstName, string lastName, IEnumerable<string> roles, IEnumerable<Claim> userClaims);
    }
}
