using Arch.EntityFrameworkCore.UnitOfWork;
using Contract.Helper;
using Contract.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SaleFishClean.Application.Common.Models.Dtos.Request;
using SaleFishClean.Application.Common.Models.Dtos.Response;
using SaleFishClean.Domains.Entities;
using SaleFishClean.Domains.Exceptions;
using SaleFishClean.Infrastructure.Data;

namespace SaleFishClean.Infrastructure.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly AppSettings _settings;
        private readonly IJwtRepository _repository;
        private readonly IUnitOfWork<SaleFishProjectContext> _unitOfWork;
        public TokenRepository(IOptions<AppSettings> setting, IJwtRepository repository, IUnitOfWork<SaleFishProjectContext> unitOfWork)
        {
            _settings = setting.Value ?? throw new ArgumentNullException(nameof(setting));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        private async Task<User> GetUserByUserName(string userName)
        {
            var user = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(predicate: u => u.UserName == userName);
            if (user == null)
            {
                throw new AppExceptions("userName invalid");
            }
            return user;
        }

        public async Task<User> GetUserByRefreshToken(string? token)
        {
            if (token == null)
            {
                return null;
            }

            var refreshToken = await _unitOfWork.GetRepository<RefreshToken>()
                                                .GetFirstOrDefaultAsync(predicate: p => p.TokenRefresh == token
                                                                        , include: i => i.Include(u => u.User));

            if (refreshToken.User == null)
            {
                return null;
            }

            return refreshToken.User;
        }
        public async Task<User> GetUserByRefreshToken2(string? token)
        {
            if (token == null)
            {
                return null;
            }
            var refreshToken = await _unitOfWork.GetRepository<RefreshToken>()
                                            .GetFirstOrDefaultAsync(predicate: p => p.TokenRefresh == token
                                                                    , include: i => i.Include(u => u.User));

            if (refreshToken.User == null)
            {
                return null;
            }

            return refreshToken.User;
        }


        private async Task RemoveOldRefreshToken(User user)
        {
            //user.RefreshTokens.RemoveAll(x =>
            //!x.IsActive &&
            //x.Created.AddDays(_settings.RefreshTokenTTL) <= DateTime.UtcNow);

            // Get all refresh token of user
            //var refreshTokens = await _unitOfWork.GetRepository<RefreshToken>()
            //                            .GetPagedListAsync(predicate: p => p.UserId == user.UserId
            //                                                                && !p.IsActive
            //                                                                && p.Created.AddDays(_settings.RefreshTokenTTL) <= DateTime.UtcNow
            //                                                , pageSize: 10000); // Default pageSize is 20

            var refreshTokens = await _unitOfWork.GetRepository<RefreshToken>()
            .GetPagedListAsync(predicate: p => p.UserId == user.UserId, pageSize: 10000);

            // Lọc các token không hoạt động và đã hết hạn trên phía client
            var expiredTokens = refreshTokens.Items
                .Where(p => !p.IsActive && p.Created.AddDays(_settings.RefreshTokenTTL) <= DateTime.UtcNow)
                .ToList();
            // Start delete by unitOfWork
            if (expiredTokens.Count > 0)
            {
                _unitOfWork.GetRepository<RefreshToken>().Delete(expiredTokens);
                await (_unitOfWork as IUnitOfWork).SaveChangesAsync();
            }
        }
        public async Task<AuthenResponse> Authenticate(AuthenRequest request)
        {
            var user = await GetUserByUserName(request.UserName);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new AppExceptions("Username or password is incorrect");
            }
            var jwtToken = _repository.GenerateJwtToken(user);
            var refreshToken = await _repository.GenerateRefreshToken(user);
            _unitOfWork.GetRepository<RefreshToken>().Insert(refreshToken);
            await (_unitOfWork as IUnitOfWork).SaveChangesAsync();
            await RemoveOldRefreshToken(user);
            return new AuthenResponse
            {
                Id = user.UserId,
                UserName = user.UserName,
                Access = true,
                Token = jwtToken,
                RefreshToken = refreshToken.TokenRefresh,
                Expired = DateTime.Now.AddMinutes(_settings.RefreshTokenTTL)
            };
        }

        public async Task<AuthenResponse> RefreshToken(string token)
        {
            var user = await GetUserByRefreshToken(token);
            var refreshToken = user.RefreshTokens.Single(x => x.TokenRefresh == token);
            if (refreshToken.IsRevoked)
            {
                RevokeDescendantRefreshTokens(refreshToken, user);
                _unitOfWork.GetRepository<User>().Update(user);
            }
            if (!refreshToken.IsActive)
                throw new AppExceptions("Invalid token");
            var newRefreshToken = await RotateRefreshToken(user, refreshToken);
            user.RefreshTokens.Add(newRefreshToken);
            await RemoveOldRefreshToken(user);
            _unitOfWork.GetRepository<User>().Update(user);
            await (_unitOfWork as IUnitOfWork).SaveChangesAsync();
            var jwtToken = _repository.GenerateJwtToken(user);
            return new AuthenResponse
            {
                Access = true,
                RefreshToken = newRefreshToken.TokenRefresh,
                Token = jwtToken,
                UserName = user.UserName,
                Id = user.UserId,
                Expired = DateTime.Now.AddMinutes(_settings.RefreshTokenTTL)
            };
        }

        private async Task<RefreshToken> RotateRefreshToken(User user, RefreshToken refreshToken)
        {
            var newRefreshToken = await _repository.GenerateRefreshToken(user);
            RevokeRefreshToken(refreshToken, newRefreshToken.TokenRefresh);
            return newRefreshToken;
        }

        private void RevokeRefreshToken(RefreshToken refreshToken, string token = null)
        {
            refreshToken.Expires = DateTime.UtcNow;
            refreshToken.ReplaceByToken = token;
        }

        private async void RevokeDescendantRefreshTokens(RefreshToken refreshToken, User user)
        {
            if (!string.IsNullOrWhiteSpace(refreshToken.ReplaceByToken))
            {
                //var childToken = _context.RefreshTokens
                //    .Where(x => x.TokenRefresh == refreshToken.ReplaceByToken)
                //    .SingleOrDefault();
                //if (childToken.IsActive)
                //    RevokeRefreshToken(childToken);
                //else
                //    RevokeDescendantRefreshTokens(childToken, user);

                var childToken = await _unitOfWork.GetRepository<RefreshToken>()
                                                    .GetFirstOrDefaultAsync(predicate: x => x.TokenRefresh == refreshToken.ReplaceByToken);

                if (childToken.IsActive)
                {
                    RevokeRefreshToken(childToken);
                }
                else
                {
                    RevokeDescendantRefreshTokens(childToken, user);
                }
            }
        }

        public async void RevokeToken(string token)
        {
            //var user = await GetUserByRefreshToken(token);
            //var refreshToken = _context.RefreshTokens.Single(x => x.TokenRefresh == token);
            //if (!refreshToken.IsActive)
            //    throw new ApplicationExcep("Invalid token");
            //RevokeRefreshToken(refreshToken);
            //_context.Update(user);
            //_context.SaveChanges();

            var user = await GetUserByRefreshToken(token);

            var refreshToken = await _unitOfWork.GetRepository<RefreshToken>().GetFirstOrDefaultAsync(predicate: x => x.TokenRefresh == token);

            if (!refreshToken.IsActive)
            {
                throw new AppExceptions("Invalid token");
            }

            RevokeRefreshToken(refreshToken);

            _unitOfWork.GetRepository<User>().Update(user);
            await (_unitOfWork as IUnitOfWork).SaveChangesAsync();

            // K hiểu lắm tại sao em lại get 2 entity độc lập và update lại user mặc dù không update gì user :D
        }
    }
}
