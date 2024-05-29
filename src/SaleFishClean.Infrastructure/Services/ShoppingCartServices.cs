using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SaleFishClean.Application.Common.Exceptions;
using SaleFishClean.Application.Common.Interfaces.Services;
using SaleFishClean.Application.Common.Models.Dtos.Response;
using SaleFishClean.Domains.Entities;
using SaleFishClean.Domains.Exceptions;
using SaleFishClean.Infrastructure.Data;
using System.Security.Claims;

namespace SaleFishClean.Infrastructure.Services
{
    public class ShoppingCartServices : RepositoryBaseAsync<SaleFishProjectContext, ShoppingCart, string>, IShoppingCartServices
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWork<SaleFishProjectContext> _unitOfWork;
        private readonly IMapper _mapper;
        public ShoppingCartServices(IUnitOfWork<SaleFishProjectContext> unitOfWork, IHttpContextAccessor contextAccessor, IMapper mapper) : base(unitOfWork)
        {
            _contextAccessor = contextAccessor;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CreateShoppingCartDetail(int productId, int? quantity)
        {
            var userId = GetUserIdFromClaim(_contextAccessor);
            try
            {
                await BeginTransactionAsync();
                var shoppingCart = await _unitOfWork.GetRepository<ShoppingCart>().GetFirstOrDefaultAsync(
                    predicate: x => x.UserId.Equals(userId));
                if (shoppingCart == null)
                {
                    shoppingCart = new ShoppingCart
                    {
                        CreateDate = DateTime.UtcNow,
                        UserId = userId,
                        CartId = CreateRandomCartId()
                    };
                    await _unitOfWork.DbContext.ShoppingCarts.AddAsync(shoppingCart);
                }

                var shoppingCartDetail = await _unitOfWork.GetRepository<ShoppingCartDetail>().GetFirstOrDefaultAsync(
                    predicate: x => x.ProductId == productId);
                if (shoppingCartDetail == null)
                {
                    if (quantity != null)
                    {
                        shoppingCartDetail = new ShoppingCartDetail
                        {
                            CartId = shoppingCart.CartId,
                            ProductId = productId,
                            Quantity = (int)quantity,
                        };
                    }
                    else
                    {
                        shoppingCartDetail = new ShoppingCartDetail
                        {
                            CartId = shoppingCart.CartId,
                            ProductId = productId,
                            Quantity = 1
                        };
                    }
                    await _unitOfWork.GetRepository<ShoppingCartDetail>().InsertAsync(shoppingCartDetail);
                }
                else
                {
                    shoppingCartDetail.Quantity += quantity ?? 1;
                    _unitOfWork.GetRepository<ShoppingCartDetail>().Update(shoppingCartDetail);
                }
                await EndTransactionAsync();
            }
            catch(Exception ex)
            {
                await RollBackTransactionAsync();
                throw ex;
            }
        }

        public async Task DeleteShoppingCartDetails(string userId, int productId)
        {
            var shoppingCartDetail = await (from detail in _unitOfWork.DbContext.ShoppingCartDetails
                                            join Cart in _unitOfWork.DbContext.ShoppingCarts on detail.CartId equals Cart.CartId
                                            where detail.ProductId == productId
                                            where Cart.UserId == userId
                                            select detail
                                            ).FirstOrDefaultAsync();
            if (shoppingCartDetail != null)
            {
                _unitOfWork.DbContext.ShoppingCartDetails.Remove(shoppingCartDetail);
                await SaveChangesAsync();
            }
        }
        public async Task DeleteAllShoppingCartDetails(string userId)
        {
            var ShoppingCart = await _unitOfWork.DbContext.ShoppingCarts
                                    .Include(x => x.ShoppingCartDetails)
                                    .Where(x => x.UserId == userId)
                                    .SingleOrDefaultAsync();
            if (ShoppingCart != null)
            {
                _unitOfWork.DbContext.ShoppingCartDetails.RemoveRange(ShoppingCart.ShoppingCartDetails);
                await SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<CartResponse>> GetAllForCartDetail()
        {
            var userid = GetUserIdFromClaim(_contextAccessor);
            List<CartResponse> cartResponses = await (
                from detail in _unitOfWork.DbContext.ShoppingCartDetails
                join cart in _unitOfWork.DbContext.ShoppingCarts on detail.CartId equals cart.CartId
                join product in _unitOfWork.DbContext.Products on detail.ProductId equals product.ProductId
                where cart.UserId == userid
                select new CartResponse
                {
                    UserId = cart.UserId,
                    Id = product.ProductId,
                    NameProduct = product.ProductName,
                    Price = product.Price,
                    Quantity = detail.Quantity,
                }).ToListAsync();
            return cartResponses ?? throw new AppExceptions("Error");
        }
        public async Task<int> CountForCartDetail()
        {
            var userid = GetUserIdFromClaim(_contextAccessor);
            List<CartResponse> cartResponses = await (
                from detail in _unitOfWork.DbContext.ShoppingCartDetails
                join cart in _unitOfWork.DbContext.ShoppingCarts on detail.CartId equals cart.CartId
                join product in _unitOfWork.DbContext.Products on detail.ProductId equals product.ProductId
                where cart.UserId == userid
                select new CartResponse
                {
                    UserId = cart.UserId,
                    Id = product.ProductId,
                    NameProduct = product.ProductName,
                    Price = product.Price,
                    Quantity = detail.Quantity,
                }).ToListAsync();
            return cartResponses.Count;
        }
        public async Task RemoveShoppingCartAsync(string userId)
        {
            var ShoppingCart = await _unitOfWork.DbContext.ShoppingCarts
                                        .Include(x => x.ShoppingCartDetails)
                                        .Where(x => x.UserId == userId)
                                        .SingleOrDefaultAsync();
            if (ShoppingCart == null)
            {
                throw new NotFoundException("ShoppingCart null");
            }
            try
            {
                foreach (var detail in ShoppingCart.ShoppingCartDetails)
                {
                    var inventory = await _unitOfWork.DbContext.Inventories
                                                .SingleOrDefaultAsync(i => i.ProductId == detail.ProductId);

                    if (inventory != null)
                    {
                        inventory.Quantity -= detail.Quantity;
                        _unitOfWork.GetRepository<Inventory>().Update(inventory);
                    }
                }
                _unitOfWork.GetRepository<ShoppingCartDetail>().Delete(ShoppingCart.ShoppingCartDetails);
                _unitOfWork.GetRepository<ShoppingCart>().Delete(ShoppingCart);
            }
            catch(Exception ex) 
            {
                throw new AppExceptions(ex.Message);
            }
        }
        public string CreateRandomCartId()
        {
            Random ran = new Random();
            string CartId = "CAR";
            for (int i = 0; i < 4; i++)
            {
                CartId += ran.Next(0, 10).ToString();
            }
            return CartId;
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
