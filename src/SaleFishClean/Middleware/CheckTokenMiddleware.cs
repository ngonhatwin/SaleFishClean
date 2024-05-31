using Contract.Interfaces;
using SaleFishClean.Application.Common.Interfaces.Services;
using SaleFishClean.Application.Common.Models.Dtos.Response;

namespace SaleFishClean.Web.Middleware
{
    public class CheckTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;
        public CheckTokenMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }
        public async Task Invoke(HttpContext context, IJwtRepository jwtRepository, IUserServices repository)
        {
            // Lấy Access Token từ cookie
            string accessToken = context.Request.Cookies["accessToken"]!;
            string accessTokenExpired = context.Request.Cookies["TokenExpried"]!;
            string userName = context.Request.Cookies["userName"]!;

            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(accessTokenExpired))
            {
                if (DateTime.TryParse(accessTokenExpired, out DateTime accessTokenExpiry))
                {
                    if (accessTokenExpiry < DateTime.Now)
                    { 
                        context.Response.Cookies.Delete("accessToken");
                        context.Response.Cookies.Delete("TokenExpried");
                        context.Response.Cookies.Delete("userId");
                        context.Response.Cookies.Delete("userName");
                    }
                    else
                    {
                        context.Request.Headers.Append("Authorization", "Bearer " + accessToken);
                        context.Session.SetString("userName", userName);
                    }
                }
            }
            if (accessToken == null)
            {
                if (context.Request.Cookies.TryGetValue("refreshToken", out string refreshToken))
                {
                    if (refreshToken != null)
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var authService = context.RequestServices.GetRequiredService<ITokenRepository>();
                            AuthenResponse authenReponse = await authService.RefreshToken(refreshToken);
                            if (authenReponse != null)
                            {
                                context.Response.Cookies.Append("refreshToken", authenReponse.RefreshToken, new CookieOptions
                                {
                                    HttpOnly = true,
                                    Expires = DateTime.UtcNow.AddDays(7)
                                });
                                accessToken = authenReponse.Token;
                                context.Request.Headers.Append("Authorization", "Bearer " + accessToken);
                                context.Response.Cookies.Append("accessToken", authenReponse.Token);
                                context.Response.Cookies.Append("TokenExpried", authenReponse.Expired.ToString());
                                context.Response.Cookies.Append("userName", authenReponse.UserName);
                                context.Response.Cookies.Append("userId", authenReponse.Id);
                                context.Session.SetString("userName", authenReponse.UserName);
                            }
                        }
                    }
                }
            }
            await _next(context);
        }
    }
}
