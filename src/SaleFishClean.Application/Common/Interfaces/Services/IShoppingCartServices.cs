using Contract.Interfaces;
using SaleFishClean.Application.Common.Models.Dtos.Response;
using SaleFishClean.Domains.Entities;


namespace SaleFishClean.Application.Common.Interfaces.Services
{
    public interface IShoppingCartServices : IRepositoryBaseAsync<ShoppingCart, string>
    {
        Task CreateShoppingCartDetail(int ProductId, int? Quantity);
        Task<int> CountForCartDetail();
        Task<IEnumerable<CartResponse>> GetAllForCartDetail();
        Task DeleteShoppingCartDetails(string userId, int productId);
        Task DeleteAllShoppingCartDetails(string userId);
        Task RemoveShoppingCartAsync(string userId);
    }
}
