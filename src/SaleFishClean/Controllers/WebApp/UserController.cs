using Contract.Extensions;
using Microsoft.AspNetCore.Mvc;
using SaleFishClean.Application.Common.Exceptions;
using SaleFishClean.Application.Common.Interfaces.Services;
using SaleFishClean.Application.Common.Models.Dtos.Request;
using ValidationException = SaleFishClean.Application.Common.Exceptions.ValidationException;

namespace SaleFishClean.Web.Controllers.WebApp
{
    public class UserController : Controller
    {
        private readonly IUserServices _service;
        private readonly IWebHostEnvironment _env;
        public UserController(IUserServices service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> CreateUser(UserRequest user)
        {
            try
            {
                var User = await _service.CreateAsync(user);
                await _service.SendOtpEmailAsync(User.Email, User.CodeOtp);
                return RedirectToAction("Otp", "WebApp");
            }
            catch (ValidationException ex)
            {
                var errorMessages = ex.Errors.Select(e => e.Value).ToList();
                return RedirectToAction("GoToRegister", "WebApp");
            }
        }

        public IActionResult IsVerify(string Otp)
        {
            int result = _service.VerifyOtp(Otp);
            if(result == 1)
            {
                TempData["ResultOpt"] = result.ToString();
            }    
            return RedirectToAction("Index", "WebApp");
        }

        [HttpPost]
        public async Task<IActionResult> AddInfoUser(UserInfoRequest request)
        {
            request.ImageName = await request.ImageFile.SaveImageAsync(_env);
            await _service.AddInfoUserAsync(request);
            return NoContent();
        }

        [HttpGet("get-user-id")]
        public async Task<string> GetUserIdByUserName(string userName)
        {
            var result = await _service.GetUserIdByUserNameAsync(userName);
            return result ?? throw new NotFoundException("Not found");
        }
    }
}
