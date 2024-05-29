using Azure;
using Contract.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SaleFishClean.Application.Common.Models.Dtos.Request;

namespace SaleFishClean.Web.Controllers.WebApp
{
    public class TokenController : Controller
    {
        private readonly ITokenRepository _repository;
        private readonly ILogger<TokenController> _logger;
        public TokenController(ITokenRepository repository, ILogger<TokenController> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        private const string MethodName = "Login";
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Login(AuthenRequest request)
        {
            _logger.LogInformation($"Begin {MethodName}");
            var result = await _repository.Authenticate(request);
            if (result == null)
            {
                return BadRequest("Authentication failed.");
            }
            HttpContext.Session.SetString("userName", result.UserName);
            setTokencookie(result.RefreshToken, result.Token, result.Expired.ToString(), result.Id, result.UserName);
            _logger.LogInformation($"End {MethodName}");
            return RedirectToAction("Index", "WebApp");
        }
        public async Task<IActionResult> RenewAsync()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken == null)
            {
                return BadRequest("refreshtoken is not at Cookies");
            }
            var result = await _repository.RefreshToken(refreshToken);
            if (result == null) { return BadRequest(result); }
            setTokencookie(result.RefreshToken, result.Token, result.Expired.ToString(), result.Id, result.UserName);
            return Ok(result);
        }
        public async Task<IActionResult> RevokeAsync(string token)
        {
            var tokenRefresh = token ?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(tokenRefresh))
                return BadRequest("Token is required");
            _repository.RevokeToken(tokenRefresh);
            return Ok(tokenRefresh);
        }
        private void setTokencookie(string refreshToken, string accessToken,
            string tokenExpired,
            string userId,
            string userName)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            var cookieOptions1 = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddMinutes(15)
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
            Response.Cookies.Append("accessToken", accessToken, cookieOptions1);
            Response.Cookies.Append("TokenExpried", tokenExpired, cookieOptions);
            Response.Cookies.Append("userId", userId, cookieOptions);
            Response.Cookies.Append("userName", userName, cookieOptions);
        }
    }
}
