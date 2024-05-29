using SaleFishClean.Application.Common.Models.Dtos.Request;
using SaleFishClean.Domains.Entities;


namespace SaleFishClean.Application.Common.Models.Dtos.Response
{
    public class IndexViewModelResponse
    {
        public IEnumerable<ProductType> ProductsType { get; set; }
        public IEnumerable<ProductResponseForUser> Products { get; set; }
        public UserInfoRequest UserInfo { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int CountShoppingCartDetails { set; get; }
    }
}
