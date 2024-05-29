using Contract.Interfaces;
using SaleFishClean.Application.Common.Models.Dtos.Request;
using SaleFishClean.Domains.Entities;


namespace SaleFishClean.Application.Common.Interfaces.Services
{
    public interface IUserServices : IRepositoryBaseAsync<User, string>
    {
        Task<User> CreateAsync(UserRequest user);
        Task SendOtpEmailAsync(string email, string otp);
        Task AddInfoUserAsync(UserInfoRequest userInfo);
        Task<Order> CreateOrderAsync();
        decimal CalculateTotalPriceOfShoppingCart(string userId);
        Task<string> GetUserNameByUserIdAsync(string userId);
        int VerifyOtp(string Otp);
        Task<UserInfoRequest> GetInfoUserAsync(string id);
        Task<string> GetUserIdByUserNameAsync(string userName);
        Task<User> GetUserByUserNameAsync(string userName);

    }
}
