using SaleFishClean.Domains.Entities;
namespace SaleFishClean.Application.Common.Models.Dtos.Response
{
    public class ShipperOrderResponse
    {
        public string OrderId { get; set; }
        public UserInfoResponse UserInfo { get; set; }
        public List<ProductDetailsResponse> ProductsForShipper { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
