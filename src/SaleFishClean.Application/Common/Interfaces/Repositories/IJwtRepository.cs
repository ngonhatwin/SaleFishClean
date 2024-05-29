using SaleFishClean.Domains.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Interfaces
{
    public interface IJwtRepository
    {
        public string GenerateJwtToken(User user);
        public string? ValidateJwtToken(string? token);
        Task<RefreshToken> GenerateRefreshToken(User user);
    }
}
