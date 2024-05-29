using Arch.EntityFrameworkCore.UnitOfWork;
using Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SaleFishClean.Application.Common.Exceptions;
using SaleFishClean.Application.Common.Interfaces.Services;
using SaleFishClean.Application.Common.Models.Dtos.Request;
using SaleFishClean.Application.Common.Models.Dtos.Response;
using SaleFishClean.Domains.Entities;
using SaleFishClean.Infrastructure.Data;
using System.Security.Claims;


namespace SaleFishClean.Infrastructure.Services
{
    public class OrderServices : RepositoryBaseAsync<SaleFishProjectContext, Order, string>, IOrderServices
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IUnitOfWork<SaleFishProjectContext> _unitOfWork;
        public OrderServices( IUnitOfWork<SaleFishProjectContext> unitOfWork, IHttpContextAccessor accessor) : base(unitOfWork)
        {
            _accessor = accessor;
            _unitOfWork = unitOfWork;
        }
        public async Task<Order> GetOrderByUserId (string userId)
        {
            var result = await _unitOfWork.DbContext.Orders.SingleOrDefaultAsync(o => o.UserId == userId && o.Status == 0);
            if( result == null ){ throw new NotFoundException("Not found order"); }
            return result;
        }
        public async Task<Order> CreateOrder()
        {
            var userId = GetUserIdFromClaim(_accessor);
            var Order = await _unitOfWork.DbContext.Orders
                .Where(x => x.UserId == userId)
                .Where(x => x.Status ==0)
                .SingleOrDefaultAsync();
            List<ShoppingCartDetail> cartResponses = await (
                from detail in _unitOfWork.DbContext.ShoppingCartDetails
                join cart in _unitOfWork.DbContext.ShoppingCarts on detail.CartId equals cart.CartId
                join product in _unitOfWork.DbContext.Products on detail.ProductId equals product.ProductId
                where cart.UserId == userId
                select detail).Include(x => x.Product).ToListAsync();
            var newOrder = new Order();
            try
            {
                await BeginTransactionAsync();
                if (Order == null || Order.Status == 1)
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
                        await _unitOfWork.DbContext.OrderDetails.AddAsync(new OrderDetail
                        {
                            OrderId = newOrder.OrderId,
                            ProductId = detail.ProductId,
                            Quantity = detail.Quantity,
                            Price = detail.Product.Price,
                        });
                    }
                    await _unitOfWork.DbContext.Orders.AddAsync(newOrder);
                    await _unitOfWork.DbContext.OrderDetails.AddRangeAsync(newOrder.OrderDetails);
                    await EndTransactionAsync();
                    return newOrder;
                }
                return Order;
            }
            catch(Exception ex) 
            {
                await RollBackTransactionAsync();
                throw ex;
            }
        }
        public async Task Update(string id)
        {
            var Order = await _unitOfWork.DbContext.Orders
                .Where(x => x.OrderId == id)
                .SingleOrDefaultAsync();
            if (Order == null){ throw new NotFoundException("Order null");}
            Order.Status = 1;
            _unitOfWork.GetRepository<Order>().Update(Order);
            await SaveChangesAsync();
        }
        public string CreateRandomId()
        {
            Random ran = new();
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
        public async Task<IEnumerable<ShipperOrderResponse>> GetOrderForShipper()
        {
            var orders = await _unitOfWork.GetRepository<Order>().GetPagedListAsync(
                predicate: x => x.Status == 1,
                include: source => source.Include(x => x.User)
                                         .Include(x => x.OrderDetails)
                                            .ThenInclude(od => od.Product));

            if (orders == null)
            {
                throw new NotFoundException("Not Found");
            }
            var orderResponses = orders.Items.Select(order => new ShipperOrderResponse
            {
                OrderId = order.OrderId,
                UserInfo = new UserInfoResponse
                {
                    UserId = order.UserId,
                    FirstName = order.FirstName,
                    LastName = order.LastName,
                    Email = order.Email,
                    PhoneNumber = order.PhoneNumber,
                    Address = order.Address,
                },
                ProductsForShipper = order.OrderDetails.Select(od => new ProductDetailsResponse
                {
                    ProductName = od.Product.ProductName,
                    Price = od.Price,
                    Quantity = od.Quantity,
                }).ToList(),
                TotalAmount = order.OrderDetails.Sum(od => od.Price * od.Quantity)
            });
            return orderResponses;
        }
        public async Task<ShipperOrderResponse> GetOrderForUser(string id)
        {
            var orders = await _unitOfWork.GetRepository<Order>().GetPagedListAsync(
                predicate: x => x.OrderId == id,
                include: source => source.Include(x => x.User)
                                         .Include(x => x.OrderDetails)
                                            .ThenInclude(od => od.Product));

            if (orders == null)
            {
                throw new NotFoundException("Not Found");
            }
            var orderResponse = orders.Items.Select(order => new ShipperOrderResponse
            {
                OrderId = order.OrderId,
                UserInfo = new UserInfoResponse
                {
                    UserId = order.UserId,
                    FirstName = order.User.FirstName,
                    LastName = order.User.LastName,
                    Email = order.User.Email,
                    PhoneNumber = order.User.PhoneNumber,
                    Address = order.User.Address,
                },
                ProductsForShipper = order.OrderDetails.Select(od => new ProductDetailsResponse
                {
                    ProductName = od.Product.ProductName,
                    Price = od.Price,
                    Quantity = od.Quantity,
                }).ToList(),
                TotalAmount = order.OrderDetails.Sum(od => od.Price * od.Quantity)
            }).FirstOrDefault();
            return orderResponse;
        }

        public async Task<IEnumerable<ShipperOrderResponse>> GetOrderShippingForShipper()
        {
            var orders = await _unitOfWork.GetRepository<Order>().GetPagedListAsync(
                predicate: x => x.Status == 2,
                include: source => source.Include(x => x.User)
                                         .Include(x => x.OrderDetails)
                                            .ThenInclude(od => od.Product));

            if (orders == null)
            {
                throw new NotFoundException("Not Found");
            }
            var orderResponses = orders.Items.Select(order => new ShipperOrderResponse
            {
                OrderId = order.OrderId,
                UserInfo = new UserInfoResponse
                {
                    UserId = order.UserId,
                    FirstName = order.FirstName,
                    LastName = order.LastName,
                    Email = order.Email,
                    PhoneNumber = order.PhoneNumber,
                    Address = order.Address,
                },
                ProductsForShipper = order.OrderDetails.Select(od => new ProductDetailsResponse
                {
                    ProductName = od.Product.ProductName,
                    Price = od.Price,
                    Quantity = od.Quantity,
                }).ToList(),
                TotalAmount = order.OrderDetails.Sum(od => od.Price * od.Quantity)
            });
            return orderResponses;
        }
        public async Task<IEnumerable<ShipperOrderResponse>> GetOrderShippedForShipper()
        {
            var orders = await _unitOfWork.GetRepository<Order>().GetPagedListAsync(
                predicate: x => x.Status == 3,
                include: source => source.Include(x => x.User)
                                         .Include(x => x.OrderDetails)
                                            .ThenInclude(od => od.Product));

            if (orders == null)
            {
                throw new NotFoundException("Not Found");
            }
            var orderResponses = orders.Items.Select(order => new ShipperOrderResponse
            {
                OrderId = order.OrderId,
                UserInfo = new UserInfoResponse
                {
                    UserId = order.UserId,
                    FirstName = order.FirstName,
                    LastName = order.LastName,
                    Email = order.Email,
                    PhoneNumber = order.PhoneNumber,
                    Address = order.Address,
                },
                ProductsForShipper = order.OrderDetails.Select(od => new ProductDetailsResponse
                {
                    ProductName = od.Product.ProductName,
                    Price = od.Price,
                    Quantity = od.Quantity,
                }).ToList(),
                TotalAmount = order.OrderDetails.Sum(od => od.Price * od.Quantity)
            });
            return orderResponses;
        }
        public async Task<IEnumerable<ShipperOrderResponse>> GetOrderDestroyForShipper()
        {
            var orders = await _unitOfWork.GetRepository<Order>().GetPagedListAsync(
                predicate: x => x.Status == 4,
                include: source => source.Include(x => x.User)
                                         .Include(x => x.OrderDetails)
                                            .ThenInclude(od => od.Product));

            if (orders == null)
            {
                throw new NotFoundException("Not Found");
            }
            var orderResponses = orders.Items.Select(order => new ShipperOrderResponse
            {
                OrderId = order.OrderId,
                UserInfo = new UserInfoResponse
                {
                    UserId = order.UserId,
                    FirstName = order.FirstName,
                    LastName = order.LastName,
                    Email = order.Email,
                    PhoneNumber = order.PhoneNumber,
                    Address = order.Address,
                },
                ProductsForShipper = order.OrderDetails.Select(od => new ProductDetailsResponse
                {
                    ProductName = od.Product.ProductName,
                    Price = od.Price,
                    Quantity = od.Quantity,
                }).ToList(),
                TotalAmount = order.OrderDetails.Sum(od => od.Price * od.Quantity)
            });
            return orderResponses;
        }
        public async Task EditInfoShip(UserInfoRequest userInfo)
        {
            var userId = GetUserIdFromClaim(_accessor);
            var user = await _unitOfWork.GetRepository<Order>().GetFirstOrDefaultAsync(
                predicate: x => x.UserId.Equals(userId) && x.Status == 0);
            if (user == null) { throw new NotFoundException($"Not found {userInfo.FirstName}"); }
            if (user != null)
            {
                user.FirstName = userInfo.FirstName;
                user.LastName = userInfo.LastName;
                user.Address = userInfo.Address;
                user.PhoneNumber = userInfo.PhoneNumber;
                user.Email = userInfo.Email;
            }
             _unitOfWork.GetRepository<Order>().Update(user);
            await (_unitOfWork as IUnitOfWork).SaveChangesAsync();
        }
        public async Task ChangeShipping(string orderId)
        {
            var order = await _unitOfWork.DbContext.Orders
                .Where(x => x.OrderId == orderId)
                .Where(x => x.Status == 1)
                .SingleOrDefaultAsync();
            var userId = GetUserIdFromClaim(_accessor);
            if (order == null)
            {
                throw new NotFoundException();
            }
            try
            {
                await BeginTransactionAsync();
                var shipper = new Shipper
                {
                    OrderID = order.OrderId,
                    ShipperId = userId,
                    CreatedAt = DateTime.Now,
                };
                await _unitOfWork.GetRepository<Shipper>().InsertAsync(shipper);
                order.Status = 2;
                _unitOfWork.GetRepository<Order>().Update(order);
                await EndTransactionAsync();
            }
            catch (Exception ex)
            {
                await RollBackTransactionAsync();
                throw ex;
            }
        }
        public async Task ChangeShipped(string orderId)
        {
            var order = await _unitOfWork.DbContext.Orders
                .Where(x => x.OrderId == orderId)
                .Where(x => x.Status == 2)
                .SingleOrDefaultAsync();
            if (order == null)
            {
                throw new NotFoundException();
            }
            order.Status = 3;
            _unitOfWork.GetRepository<Order>().Update(order);
            await (_unitOfWork as IUnitOfWork).SaveChangesAsync();
        }
        public async Task ChangeDestroy(string orderId)
        {
            var order = await _unitOfWork.DbContext.Orders
                .Where(x => x.OrderId == orderId)
                .Where(x => x.Status == 2)
                .SingleOrDefaultAsync();
            if (order == null)
            {
                throw new NotFoundException();
            }
            order.Status = 4;
            _unitOfWork.GetRepository<Order>().Update(order);
            await (_unitOfWork as IUnitOfWork).SaveChangesAsync();
        }
        public async Task<IEnumerable<OrderTrackingViewModel>> GetOrderTracking(string userId)
        {
            var orders = await _unitOfWork.GetRepository<Order>().GetPagedListAsync(
                predicate: x => x.UserId == userId,
                include: x => x.Include(x => x.OrderDetails)
                               .Include(x => x.Shippers)
                               .ThenInclude(x => x.User));
            var orderTrackingViewModels = orders.Items.Select(order => new OrderTrackingViewModel
            {
                OrderId = order.OrderId,
                BuyerName = order.FirstName + " " + order.LastName,
                ShipperName = order.Shippers.FirstOrDefault()?.User.FirstName + " " + order.Shippers.FirstOrDefault()?.User.LastName,
                TotalAmount = order.OrderDetails.Sum(od => od.Price * od.Quantity),
                Status = order.Status
            });
            return orderTrackingViewModels;
        }
    }
}
