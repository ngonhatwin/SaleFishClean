using SaleFishClean.Application.Common.Models.Dtos.Request;
using SaleFishClean.Application.Common.Models.Dtos.Response;
using SaleFishClean.Domains.Entities;

namespace Contract.Interfaces
{
    public interface ITokenRepository
    {
        Task<AuthenResponse> Authenticate(AuthenRequest request);
        Task<AuthenResponse> RefreshToken(string token);
        void RevokeToken(string token);
        Task<User> GetUserByRefreshToken(string? token);
        Task<User> GetUserByRefreshToken2(string? token);
    }

}
