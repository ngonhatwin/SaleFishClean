using Contract.Helper;
using Contract.Interfaces;
using Microsoft.Extensions.Options;
using SaleFishClean.Application.Common.Interfaces.Services;

namespace SaleFishClean.Web.Middleware
{
    public class AddTokenToHeaderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public AddTokenToHeaderMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }
        public async Task Invoke(HttpContext context, IUserServices repository, IJwtRepository jwtrepository)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = jwtrepository.ValidateJwtToken(token);
            if (userId != null)
            {
                // attach user to context on successful jwt validation
                context.Items["user"] = await repository.GetByIdAsync(userId);
            }
            await _next(context);
        }
    }
}
