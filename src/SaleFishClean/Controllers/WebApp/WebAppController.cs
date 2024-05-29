
using Contract.Extensions;
using Contract.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaleFishClean.Application.Common.Interfaces.Services;
using SaleFishClean.Application.Common.Models.Dtos.Request;
using SaleFishClean.Application.Common.Models.Dtos.Response;
using SaleFishClean.Domains.Entities;
using SaleFishClean.Infrastructure;
using StackExchange.Redis;
using System.Security.Claims;

namespace SaleFishClean.Web.Controllers.WebApp
{
    public class WebAppController(IProductServices service,
        IVnPayServices vnPayService, IUserServices userService,
        ITokenRepository tokenRepository, IOrderServices orderServices,
        IShoppingCartServices shoppingCartServices,
        IWebHostEnvironment env,
        IHttpContextAccessor accessor) : Controller
    {
        private readonly IProductServices _productService = service;
        private readonly IVnPayServices _vnPayService = vnPayService;
        private readonly IUserServices _userService = userService;
        private readonly ITokenRepository _tokenRepository = tokenRepository;
        private readonly IShoppingCartServices _shoppingCartServices = shoppingCartServices;
        private readonly IOrderServices _orderServices = orderServices;
        private readonly IWebHostEnvironment _env = env;
        private readonly IHttpContextAccessor _accessor = accessor;
        public int PageSize = 6;
        public async Task<IActionResult> Index(int page = 0, UserInfoRequest? request = null)
        {
            var productTypes = await _productService.GetProductTypesAsync();
            var refreshToken = Request.Cookies["refreshToken"];
            var user = await _tokenRepository.GetUserByRefreshToken2(refreshToken);
            var products = await _productService.GetPagedAsync(page);
            var totalProducts = await _productService.GetTotalProductsAsync();
            var totalPages = (int)Math.Ceiling((double)totalProducts / PageSize);
            request.ImageName = await request.ImageFile.SaveImageAsync(_env);
            var countProductInCart = 0;
            if (User == null)
            {
                ViewData["Name"] = null;
                HttpContext.Session.SetString("IsLoggedIn", "false");
                countProductInCart = 0;
            }
            else
            {
                var isShipper = User.IsInRole("shipper");
                if (isShipper)
                {
                    HttpContext.Session.SetString("IsLoggedIn", "true");
                    HttpContext.Session.SetString("NameShipper", user.UserName);
                    return RedirectToAction("GetOrderForShipper", "Shipper");
                }
                bool isAuthen = HttpContext.User.Identity.IsAuthenticated;
                if (isAuthen == true)
                {
                    HttpContext.Session.SetString("IsLoggedIn", "true");
                    ViewData["Name"] = user.UserName;
                    request = await _userService.GetInfoUserAsync(user.UserId);
                    
                    countProductInCart = await _shoppingCartServices.CountForCartDetail();
                }
                else
                {
                    HttpContext.Session.SetString("IsLoggedIn", "false");
                    countProductInCart = 0;
                }
            }
            ViewData["Count"] = countProductInCart.ToString();
            var IndexView = new IndexViewModelResponse
            {
                UserInfo = request,
                Products = products,
                ProductsType = productTypes,
                TotalPages = totalPages,
                CurrentPage = page,
            };
            return View(IndexView);
        }
        [Route("productDetails")]
        public async Task<IActionResult> GoToProductDetails(int id)
        {
            bool isAuthen = HttpContext.User.Identity.IsAuthenticated;
            var userId = GetUserIdFromClaim(_accessor);
            var countProductInCart = await _shoppingCartServices.CountForCartDetail();
            var product = await _productService.GetProductDetailsAsync(id);
            var userName = await _userService.GetUserNameByUserIdAsync(userId);
            if (userName == null)
            {
                countProductInCart = await _shoppingCartServices.CountForCartDetail();
                ViewData["Count"] = countProductInCart.ToString();
                ViewData["userId"] = userId;
                ViewData["userName"] = userName;
                product = await _productService.GetProductDetailsAsync(id);
                return View("ProductDetails", product);
            }
            if (isAuthen)
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
            }
            ViewData["Count"] = countProductInCart.ToString();
            ViewData["userId"] = userId;
            ViewData["userName"] = userName;
            
            return View("ProductDetails", product);
        }

        [Route("GoToLogin")]
        public IActionResult GoToLogin()
        {
            return View("Login");
        }
        [Route("register")]
        public IActionResult GoToRegister()
        {
            return View("Register");
        }
        [Route("Otp")]
        public IActionResult OTP()
        {
            return View("OTP");
        }
        [Route("Cart")]

        public IActionResult GoToCart()
        {
            return View("Cart");
        }
        [Route("add-info-user")]
        public async Task<IActionResult> AddInfo()
        {
            await _orderServices.CreateOrder();
            var Id = GetUserIdFromClaim(_accessor);
            var userInfo = await _userService.GetInfoUserAsync(Id);
            return View("InfoUser", userInfo);
        }
        public IActionResult Logout()
        {
            Response.Cookies.Delete("TokenExpried");
            Response.Cookies.Delete("refreshToken");
            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("userId");
            Response.Cookies.Delete("userName");
            return RedirectToAction("Index", "WebApp");
        }
        public IActionResult PaymentFail()
        {
            return View("PaymentFail");
        }
        public IActionResult PaymentSuccess()
        {
            return View("PaymentSuccess");
        }
        public async Task<IActionResult> PaymentCallBack()
        {
            var userId = GetUserIdFromClaim(_accessor);
            var Response = _vnPayService.PaymentExecute(Request.Query);
            if (Response == null || Response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Đơn hàng thanh toán thất bại mã lỗi {Response.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }
            try
            {
                await _orderServices.BeginTransactionAsync();
                await _shoppingCartServices.RemoveShoppingCartAsync(userId);
                await _orderServices.Update(Response.OrderId);
                await _orderServices.EndTransactionAsync();
                TempData["Message"] = $"Đơn hàng thanh toán thành công";
                return RedirectToAction("PaymentSuccess");
            }
            catch
            {
                await _orderServices.RollBackTransactionAsync();
                TempData["Message"] = $"Đơn hàng thanh toán thất bại mã lỗi {Response.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }
        }
    
        [Route("Search")]
        public async Task<IActionResult> Search(string? name, UserInfoRequest? Request, int pageIndex)
        {
            var countProductInCart = await _shoppingCartServices.CountForCartDetail();
            ViewData["Count"] = countProductInCart.ToString();
            var userid = GetUserIdFromClaim(_accessor);
            var request = Request;
            if (userid != null)
            {
                request = await _userService.GetInfoUserAsync(userid);
            }
            var products = await _productService.GetProductsAsync(null, null, name, null, pageIndex);
            var totalCount = products.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / PageSize);
            var productTypes = await _productService.GetProductTypesAsync();
            var viewModel = new IndexViewModelResponse
            {
                CurrentPage = pageIndex,
                TotalPages = totalPages,
                Products = products,
                ProductsType = productTypes,
                UserInfo = request
            };
            return View("Index", viewModel);
        }


        [Route("search-by-type")]
        public async Task<IActionResult> SearchByType(string type, UserInfoRequest? request, int pageIndex)
        {
            var countProductInCart = await _shoppingCartServices.CountForCartDetail();
            ViewData["Count"] = countProductInCart.ToString();
            var userId = GetUserIdFromClaim(_accessor);
            if (userId != null)
            {
                request = await _userService.GetInfoUserAsync(userId);
            }
            var products = await _productService.GetProductsAsync(null, null, null, type, pageIndex);
            if(products.Count() == 0)
            {
                ViewData["product"] = "Notfound";
            }    
            var totalCount = await _productService.CountProductByTypeAsync(type);
            var totalPages = (int)Math.Ceiling((double)totalCount / PageSize);
            var productTypes = await _productService.GetProductTypesAsync();
            var viewModel = new IndexViewModelResponse
            {
                Products = products,
                TotalPages = totalPages,
                CurrentPage = pageIndex,
                ProductsType = productTypes,
                UserInfo = request
            };
            return View("Index", viewModel);
        }


        [Route("sort")]
        public async Task<IActionResult> Sort(string? name, string? price, int pageIndex, UserInfoRequest request)
        {
            var countProductInCart = await _shoppingCartServices.CountForCartDetail();
            ViewData["Count"] = countProductInCart.ToString();
            var userid = GetUserIdFromClaim(_accessor);
            if (userid != null)
            {
                request = await _userService.GetInfoUserAsync(userid);
            }
            var products = await _productService.GetProductsAsync(name, price, null, null, pageIndex);
            var totalCount = await _productService.GetAllForUserAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount.Count() / PageSize);
            var productTypes = await _productService.GetProductTypesAsync();
            var viewModel = new IndexViewModelResponse
            {
                Products = products,
                ProductsType = productTypes,
                CurrentPage = pageIndex,
                TotalPages = totalPages,
                UserInfo = request,
            };
            return View("Index", viewModel);
        }
        [HttpGet("paging")]
        public async Task<IActionResult> Page(int page, UserInfoRequest request, string? type)
        {
            var countProductInCart = await _shoppingCartServices.CountForCartDetail();
            ViewData["Count"] = countProductInCart.ToString();
            var userid = GetUserIdFromClaim(_accessor);
            if (userid != null)
            {
                request = await _userService.GetInfoUserAsync(userid);
            }
            var productsInOnePage = await _productService.GetPagedAsync(page);
            var totalCount = await _productService.GetTotalProductsAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / PageSize);
            var productTypes = await _productService.GetProductTypesAsync();
            var viewModel = new IndexViewModelResponse
            {
                UserInfo = request,
                Products = productsInOnePage,
                CurrentPage = page,
                TotalPages = totalPages,
                ProductsType = productTypes,
            };
            return View("Index", viewModel);
        }


        [HttpPost("add-to-cart")]
        [Authorize(Roles = "user")]
        public async Task<int> AddToCart(int productId, int? Quantity)
        {
            await _shoppingCartServices.CreateShoppingCartDetail(productId, Quantity);
            var countProductInCart = await _shoppingCartServices.CountForCartDetail();
            ViewData["Count"] = countProductInCart.ToString();
            return countProductInCart;
        }
        [HttpGet("get-to-cart")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetToCart()
        {
            var result = await _shoppingCartServices.GetAllForCartDetail();
            return View("Cart", result);
        }
        [HttpPost("delete-cart")]
        public async Task<IActionResult> DeleteCartDetail(string userId, int productId)
        {
            await _shoppingCartServices.DeleteShoppingCartDetails(userId, productId);
            return RedirectToAction("GetToCart", "WebApp");
        }
        [HttpPost("delete-all-cartdetails")]
        public async Task<IActionResult> DeleteAllCartDetail()
        {
            var userId = GetUserIdFromClaim(_accessor);
            await _shoppingCartServices.DeleteAllShoppingCartDetails(userId);
            return RedirectToAction("GetToCart", "WebApp");
        }
        [HttpPost("payment")]
        public async Task<IActionResult> Payment(UserInfoRequest Request, string Payment = "VnPay")
        {
            await _orderServices.EditInfoShip(Request);
            var userid = GetUserIdFromClaim(_accessor);
            var order = await _orderServices.GetOrderByUserId(userid);
            if (Payment == "VnPay")
            {
                var VnPaymentRequest = new VnPaymentRequest
                {
                    TotalAmount = _userService.CalculateTotalPriceOfShoppingCart(userid),
                    CreatedDate = DateTime.Now,
                    FullName = $"{Request.FirstName} {Request.LastName}",
                    Description = "Thanh toán Mua Hàng",
                    OrderId = order.OrderId
                };
                return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, VnPaymentRequest));
            }
            return NoContent();
        }
        /// <summary>
        /// Controller for Shipper
        /// </summary>
        /// <param name="_contextAccessor"></param>
        /// <returns></returns>

        public async Task<IActionResult> GetAllOrderTracking()
        {
            bool isAuthen = HttpContext.User.Identity.IsAuthenticated;
            var userId = GetUserIdFromClaim(_accessor);
            var order = await _orderServices.GetOrderTracking(userId);
            if (order == null)
            {
                return NotFound();
            }
            if (isAuthen == true)
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                return View("OrderTrackingViewModel", order);
            }
            return RedirectToAction("GoToLogin", "WebApp");
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
