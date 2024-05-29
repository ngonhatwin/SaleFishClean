using Contract;
using Contract.Interfaces;
using SaleFishClean.Application.Common.Models.Dtos.Response;
using SaleFishClean.Domains.Entities;


namespace SaleFishClean.Application.Common.Interfaces.Services
{
    public interface IProductServices : IRepositoryBaseAsync<Product, int>
    {
        Task<Product> GetDetailAsync(int id);
        Task<Product> GetProductWithMaxIdAsync();   
        Task<IEnumerable<ProductResponseForUser>> GetAllForUserAsync();
        Task<IEnumerable<ProductType>> GetProductTypesAsync();  
        Task<IEnumerable<ProductResponseForUser>> GetPagedAsync(int PageIndex);  
        Task<ProductDetailsResponse> GetProductDetailsAsync(int id);
        Task<int> CountProductByTypeAsync(string productType);
        Task<bool> ProductExistsAsync(int id);
        Task<int> GetTotalProductsAsync();
        Task<string> GetProductNameByProductIdAsync(int id);
        Task<IEnumerable<ProductResponseForUser>> GetProductsAsync(string? sortName = null, string? sortPrice = null, string? name = null, string? type = null, int pageIndex = 0);
    }
}
