using Contract.Interfaces;
using SaleFishClean.Application.Common.Models.Dtos.Request;
using SaleFishClean.Application.Common.Models.Dtos.Response;
using SaleFishClean.Domains.Entities;
namespace SaleFishClean.Application.Common.Interfaces.Services
{
    public interface IOrderServices : IRepositoryBaseAsync<Order, string>
    {
        Task<Order> CreateOrder();
        Task Update(string id);
        Task<IEnumerable<ShipperOrderResponse>> GetOrderForShipper();
        Task<ShipperOrderResponse> GetOrderForUser(string id);
        Task EditInfoShip(UserInfoRequest userInfo);
        Task<Order> GetOrderByUserId(string userId);
        Task ChangeShipping(string orderId);
        Task ChangeShipped(string orderId);
        Task ChangeDestroy(string orderId);
        Task<IEnumerable<ShipperOrderResponse>> GetOrderShippingForShipper();
        Task<IEnumerable<ShipperOrderResponse>> GetOrderShippedForShipper();
        Task<IEnumerable<ShipperOrderResponse>> GetOrderDestroyForShipper();
        Task<IEnumerable<OrderTrackingViewModel>> GetOrderTracking(string userId);
       
    }
}
