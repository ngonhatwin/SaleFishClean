using Arch.EntityFrameworkCore.UnitOfWork;
using Contract.Helper;
using Contract.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SaleFishClean.Domains.Entities;
using SaleFishClean.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SaleFishClean.Infrastructure.Repositories
{
    public class JwtRepository : IJwtRepository
    {
        private readonly AppSettings _appSettings;
        private readonly IUnitOfWork<SaleFishProjectContext> _unitOfWork;
        public JwtRepository(IOptions<AppSettings> setting, IUnitOfWork<SaleFishProjectContext> unitOfWork)
        {
            _appSettings = setting.Value;
            if (string.IsNullOrEmpty(_appSettings.SecretKey))
            {
                throw new Exception("Jwt secret not configured");
            }
            _unitOfWork = unitOfWork;
        }
        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.SecretKey!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Email", user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId),
                    new Claim("id", user.UserId),
                    new Claim("IsAuthenticated", "true"),
                    new Claim(ClaimTypes.Role, user.RollName),
                }),
                Expires = DateTime.Now.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<RefreshToken> GenerateRefreshToken(User user)
        {
            var refreshToken = new RefreshToken
            {
                UserId = user.UserId,
                TokenRefresh = await getUniqueToken(),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now,
            };
            return refreshToken;
            async Task<string> getUniqueToken()
            {
                var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                var tokens = await _unitOfWork.GetRepository<RefreshToken>().GetPagedListAsync(predicate: p => p.TokenRefresh == token
                                                                                , pageSize: 10000);
                if (tokens.TotalCount > 0)
                {
                    return await getUniqueToken();
                }
                return token;
            }
        }

        public string? ValidateJwtToken(string? token)
        {
            if (token == null)
                return null;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.SecretKey!);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == "id").Value;
                return userId;
            }
            catch
            (Exception ex)
            {
                return null;
            }
        }
    }
}
