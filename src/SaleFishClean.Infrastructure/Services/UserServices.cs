using Arch.EntityFrameworkCore.UnitOfWork;
using Contract;
using Contract.Common.Interfaces;
using Contract.Helper;
using FluentValidation;
using FluentValidation.Results;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;
using SaleFishClean.Application.Common.Exceptions;
using SaleFishClean.Application.Common.Interfaces.Services;
using SaleFishClean.Application.Common.Models.Dtos.Request;
using SaleFishClean.Domains.Entities;
using SaleFishClean.Domains.Exceptions;
using SaleFishClean.Infrastructure.Data;
using System.Security.Claims;
using ValidationException = SaleFishClean.Application.Common.Exceptions.ValidationException;

namespace SaleFishClean.Infrastructure.Services
{
    public class UserServices : RepositoryBaseAsync<SaleFishProjectContext, User, string>, IUserServices
    {
        private readonly MailSettings _mailsettings;
        private readonly IDistributedCache _redisCacheService;
        private readonly ISerializeService _serializeService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWork<SaleFishProjectContext> _unitOfWork;
        private readonly IValidator<UserRequest> _validator;
        public UserServices(IOptions<MailSettings> mailSettings,
            IUnitOfWork<SaleFishProjectContext> unitOfWork,
            IHttpContextAccessor contextAccessor,
            ISerializeService serializeService,
            IDistributedCache redisCacheService,
            IValidator<UserRequest> validator) : base(unitOfWork)
        {
            _contextAccessor = contextAccessor;
            _serializeService = serializeService;
            _redisCacheService = redisCacheService;
            _mailsettings = mailSettings.Value;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<User> CreateAsync(UserRequest userRequest)
        {
            ValidationResult checkUserRequest = await _validator.ValidateAsync(userRequest);
            if(!checkUserRequest.IsValid) { throw new ValidationException(checkUserRequest.Errors); }

            var existingUser = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(
                predicate: x => x.UserName.Equals(userRequest.UserName));
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userRequest.Password, BCrypt.Net.BCrypt.GenerateSalt());
            if (existingUser != null)
            {
                throw new AppExceptions("User already exists!");
            }
            var newUser = new User
            {
                UserName = userRequest.UserName,
                UserId = CreateRandomId(),
                PasswordHash = passwordHash,
                Email = userRequest.Email,
                CodeOtp = CreateRandomOTP(),
                OtpExpired = DateTime.Now.AddMinutes(5),
                OtpCreate = DateTime.Now,
                IsVerify = false,
                RollName = "user",
            };
            await _unitOfWork.GetRepository<User>().InsertAsync(newUser);
            await SaveChangesAsync();
            return newUser;
        }
        public async Task<Order> CreateOrderAsync()
        {
            var userId = GetUserIdFromClaim(_contextAccessor);
            var order = await _unitOfWork.DbContext.Orders
                .Where(x => x.UserId == userId)
                .SingleOrDefaultAsync();

            List<ShoppingCartDetail> cartResponses = await (
                from detail in _unitOfWork.DbContext.ShoppingCartDetails
                join cart in _unitOfWork.DbContext.ShoppingCarts on detail.CartId equals cart.CartId
                join product in _unitOfWork.DbContext.Products on detail.ProductId equals product.ProductId
                where cart.UserId == userId
                select detail).ToListAsync();
            var newOrder = new Order();
            if (order == null)
            {
                newOrder = new Order
                {
                    OrderId = CreateRandomId(),
                    OrderDate = DateTime.Now,
                    UserId = userId,
                    PaymentMethod = "VnPay",
                    Status = 0
                };
                foreach (var detail in cartResponses)
                {
                    await _unitOfWork.GetRepository<OrderDetail>().InsertAsync(new OrderDetail
                    {
                        OrderId = newOrder.OrderId,
                        ProductId = detail.ProductId,
                    });
                }
                await _unitOfWork.GetRepository<Order>().InsertAsync(newOrder);
                await _unitOfWork.GetRepository<OrderDetail>().InsertAsync(newOrder.OrderDetails);
                await SaveChangesAsync();
                return newOrder;
            }
            return order;
        }
        public decimal CalculateTotalPriceOfShoppingCart(string userId)
        {
            var shoppingCartDetail =  _unitOfWork.GetRepository<ShoppingCartDetail>().GetPagedList(
                predicate: x => x.Cart.UserId.Equals(userId),
                include: x => x.Include(y => y.Product));

            decimal totalPrice = 0;
            foreach (var detail in shoppingCartDetail.Items)
            {

                decimal productPrice = detail.Product.Price;
                int quantity = detail.Quantity;
                totalPrice += productPrice * quantity;
            }
            return  totalPrice;
        }

        public async Task AddInfoUserAsync(UserInfoRequest userInfo)
        {
            var userId = GetUserIdFromClaim(_contextAccessor);
            var user = await _unitOfWork.GetRepository<User>().GetFirstOrDefaultAsync(
                predicate: x => x.UserId.Equals(userId));
            if(user == null){ throw new NotFoundException($"Not found {userInfo.FirstName}");}
            if (user != null)
            {
                user.FirstName = userInfo.FirstName;
                user.LastName = userInfo.LastName;
                user.Address = userInfo.Address;
                user.PhoneNumber = userInfo.PhoneNumber;
                user.Cccd = userInfo.Cccd;
                user.ImageName = userInfo.ImageName;
                user.Email = userInfo.Email;
            }
            await _redisCacheService.SetStringAsync(userId, _serializeService.Serialize(new UserInfoRequest
            {
                Email = userInfo.Email,
                Address = userInfo.Address,
                Cccd = userInfo.Cccd,
                FirstName = userInfo.FirstName,
                ImageName = userInfo.ImageName,
                LastName = userInfo.LastName,
                PhoneNumber = userInfo.PhoneNumber,
            }));
            _unitOfWork.GetRepository<User>().Update(user);
            await SaveChangesAsync();
        }

        public async Task<string> GetUserNameByUserIdAsync(string userId)
        {
            var result = await _unitOfWork.DbContext.Users
                    .Where(x => x.UserId == userId)
                    .SingleOrDefaultAsync();
            return result?.UserName ;
        }

        public async Task<string> GetUserIdByUserNameAsync(string userName)
        {
            var User = await _unitOfWork.DbContext.Users
                .Where(x => x.UserName == userName)
                .SingleOrDefaultAsync();
            return User?.UserId ?? throw new AppExceptions("not found");
        }

        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            if (userName == null)
            {
                throw new ArgumentNullException(nameof(userName));
            }
            var user = await _unitOfWork.DbContext.Users
                .Where(x => x.UserName == userName)
                .SingleOrDefaultAsync();
            return user ?? throw new NotFoundException("Not found");
        }

        public async Task<UserInfoRequest> GetInfoUserAsync(string id)
        {
            var userInfoRequest = new UserInfoRequest();
            var userInRedis = await _redisCacheService.GetStringAsync(id);

            if (userInRedis == null)
            {
                var user = await _unitOfWork.DbContext.Users
                    .Where(x => x.UserId == id)
                    .SingleOrDefaultAsync();
                if (user != null)
                {
                    userInfoRequest = new UserInfoRequest
                    {
                        Email = user.Email,
                        Address = user.Address,
                        Cccd = user.Cccd,
                        FirstName = user.FirstName,
                        ImageName = user.ImageName,
                        LastName = user.LastName,
                        PhoneNumber = user.PhoneNumber
                    };
                    await _redisCacheService.SetStringAsync(id, _serializeService.Serialize(userInfoRequest));
                }
                else
                {
                    throw new NotFoundException("Not found user"); 
                }
            }
            else
            {   // Nếu có trong Redis, trả về thông tin từ Redis
                var user = JsonConvert.DeserializeObject<User>(userInRedis);
                // Thiết lập thông tin user để trả về
                userInfoRequest = new UserInfoRequest
                {
                    Email = user.Email,
                    Address = user.Address,
                    Cccd = user.Cccd,
                    FirstName = user.FirstName,
                    ImageName = user.ImageName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber
                };
            }
            return userInfoRequest;
        }

        public async Task SendOtpEmailAsync(string email, string otp)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_mailsettings.SenderName, _mailsettings.SenderEmail));
            message.To.Add(new MailboxAddress(_mailsettings.SenderName, email));
            message.Subject = "Your OTP for Password Reset";

            message.Body = new TextPart("plain")
            {
                Text = $"Your OTP for password reset is: {otp}. Please use this code within 5 minutes.",
            };
            using var client = new MailKit.Net.Smtp.SmtpClient();
            await client.ConnectAsync(_mailsettings.Server, _mailsettings.Port, SecureSocketOptions.Auto);
            await client.AuthenticateAsync(_mailsettings.UserName, _mailsettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public int VerifyOtp(string Otp)
        {
            if (string.IsNullOrEmpty(Otp))
            {
                throw new AppExceptions("Otp is null");
            }
            var User = _unitOfWork.DbContext.Users
                .Where(x => x.CodeOtp == Otp)
                .FirstOrDefault();
            if (User == null)
            {
                throw new AppExceptions("Otp invalid");
            }
            User.IsVerify = true;
            _unitOfWork.DbContext.SaveChangesAsync();
            return 1;
        }

        public string CreateRandomOTP()
        {
            Random ran = new Random();
            string randomOTP = "";
            for (int i = 0; i < 5; i++)
            {
                randomOTP += ran.Next(0, 10).ToString();
            }
            return randomOTP;
        }

        public string CreateRandomId()
        {
            Random ran = new Random();
            string Id = "";
            for (int i = 0; i < 5; i++)
            {
                Id += ran.Next(0, 10).ToString();
            }
            return Id;
        }

        public string GetUserIdFromClaim(IHttpContextAccessor _contextAccessor)
        {
            ClaimsPrincipal user = _contextAccessor.HttpContext.User;
            string userId = null;
            if (user != null && user.Identity.IsAuthenticated)
            {
                Claim userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    userId = userIdClaim.Value;
                }
            }
            return userId;
        }
    }
}
