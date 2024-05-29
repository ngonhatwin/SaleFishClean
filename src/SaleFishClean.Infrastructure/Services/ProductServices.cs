using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using Contract;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SaleFishClean.Application.Common.Exceptions;
using SaleFishClean.Application.Common.Interfaces.Services;
using SaleFishClean.Application.Common.Models.Dtos.Response;
using SaleFishClean.Domains.Entities;
using SaleFishClean.Domains.Exceptions;
using SaleFishClean.Infrastructure.Data;

using System.Net.WebSockets;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
namespace SaleFishClean.Infrastructure.Services
{
    public class ProductServices : RepositoryBaseAsync<SaleFishProjectContext, Product, int>, IProductServices
    {
        private readonly IUnitOfWork<SaleFishProjectContext> _unitOfWork;
        private readonly IMapper _mapper;

        public ProductServices(IUnitOfWork<SaleFishProjectContext> unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> CountProductByTypeAsync(string productType)
        {
            var result = await _unitOfWork.GetRepository<Product>().GetPagedListAsync(predicate: x => x.ProductType.ProductTypeName == productType);
            return result.TotalCount;
        }

        public async Task<IEnumerable<ProductResponseForUser>> GetAllForUserAsync()
        {
            var products = await _unitOfWork.GetRepository<Product>().GetPagedListAsync();
            var result = _mapper.Map<IEnumerable<ProductResponseForUser>>(products);
            return result;
        }

        public async Task<Product> GetDetailAsync(int id)
        {
            var products = await _unitOfWork.GetRepository<Product>().GetFirstOrDefaultAsync(
                predicate: x => x.ProductId.Equals(id), 
                include: source => source.Include(p => p.Brand)
                                         .Include(p => p.Discount)
                                         .Include(p => p.ProductType));
            return products ?? throw new NotFoundException($"Not found {products?.ProductName}");
        }

        public async Task<ProductDetailsResponse> GetProductDetailsAsync(int id)
        {
            var productInventory = await _unitOfWork.GetRepository<Inventory>().GetFirstOrDefaultAsync(
                predicate: x => x.ProductId.Equals(id));
            var productEntity = await _unitOfWork.GetRepository<Product>().GetFirstOrDefaultAsync(
                predicate: x => x.ProductId.Equals(id));
            if (productEntity == null){ throw new NotFoundException("Not found");}
            var productResponse = _mapper.Map<ProductDetailsResponse>(productEntity);
            productResponse.Quantity = (int)(productInventory?.Quantity ?? 0);

            return productResponse;
        }
        public async Task<Product> GetProductWithMaxIdAsync()
        {
            var maxProductId = await _unitOfWork.DbContext.Products.MaxAsync(p => (int?)p.ProductId);
            if (maxProductId == null) { throw new NotFoundException("Not found"); }
            var product = await _unitOfWork.GetRepository<Product>().GetFirstOrDefaultAsync(
                predicate: x => x.ProductId.Equals(maxProductId));
            return product;
        }

        public async Task<IEnumerable<ProductType>> GetProductTypesAsync()
        {
            var productTypes = await _unitOfWork.GetRepository<ProductType>().GetPagedListAsync();
            return productTypes.Items;
        }

        public async Task<int> GetTotalProductsAsync()
        {
            var total = await _unitOfWork.DbContext.Products.CountAsync();
            if(total == 0){ throw new AppExceptions("Table have not record"); }    
            return total;
        }

        public async Task<string> GetProductNameByProductIdAsync(int id)
        {
            var product = await _unitOfWork.GetRepository<Product>().FindAsync(id);
            return product.ProductName ?? throw new NotFoundException();
        }
        public async Task<IEnumerable<ProductResponseForUser>> GetProductsAsync( string? sortName = null, string? sortPrice = null, string? name = null, string? type = null, int pageIndex = 0)
        {
            IQueryable<Product> productQuery = _unitOfWork.GetRepository<Product>().GetAll();

            if (!string.IsNullOrEmpty(name))
            {
                productQuery = productQuery.Where(x => x.ProductName.Contains(name));
            }

            if (!string.IsNullOrEmpty(type))
            {
                productQuery = productQuery.Where(x => x.ProductType.ProductTypeName.Equals(type));
            }

            if (!string.IsNullOrEmpty(sortName))
            {
                productQuery = sortName == "Z" ? productQuery.OrderByDescending(x => x.ProductName) : productQuery.OrderBy(x => x.ProductName);
            }

            if (!string.IsNullOrEmpty(sortPrice))
            {
                productQuery = sortPrice == "min" ? productQuery.OrderBy(x => x.Price) : productQuery.OrderByDescending(x => x.Price);
            }
            var productPagedList = await _unitOfWork.GetRepository<Product>().GetPagedListAsync(
                predicate: null,
                orderBy: source => (IOrderedQueryable<Product>)productQuery,
                pageSize: 6,
                pageIndex: pageIndex
            );
            var result = _mapper.Map<IEnumerable<ProductResponseForUser>>(productPagedList.Items);
            return result;
        }

        public async Task<IEnumerable<ProductResponseForUser>> GetPagedAsync(int PageIndex)
        {
            var products = await _unitOfWork.GetRepository<Product>().GetPagedListAsync(pageIndex: PageIndex, pageSize: 6);
            var result = _mapper.Map<IEnumerable<ProductResponseForUser>>(products);
            return result;
        }
        //public async Task<IEnumerable<ProductResponseForUser>> SearchAsync(string name, string? type, int pageIndex)
        //{
        //    var products = await _unitOfWork.GetRepository<Product>().GetPagedListAsync(
        //        predicate: x => x.ProductName.Contains(name), pageIndex: pageIndex, pageSize: 6);
        //    var result = _mapper.Map<IEnumerable<ProductResponseForUser>>(products);
        //    return result;
        //}

        //public async Task<IEnumerable<ProductResponseForUser>> SearchByTypeAsync(string type, int pageIndex)
        //{
        //    var products = await _unitOfWork.GetRepository<Product>().GetPagedListAsync(
        //        predicate: u => u.ProductType.ProductTypeName.Equals(type), pageSize: 6, pageIndex: pageIndex);
        //    if(products == null){ throw new NotFoundException("Not found"); }    
        //    var result = _mapper.Map<IEnumerable<ProductResponseForUser>>(products);
        //    return result;
        //}

        //public async Task<IEnumerable<ProductResponseForUser>> SortAsync(string? name, string? price, string? priceDes, int pageIndex)
        //{
        //    IQueryable<Product> productQuery = _unitOfWork.GetRepository<Product>().GetAll();

        //    productQuery = name == "Z" ? productQuery.OrderByDescending(x => x.ProductName) : productQuery.OrderBy(x => x.ProductName);
        //    productQuery = price == "min" ? productQuery.OrderBy(x => x.Price) : productQuery;
        //    productQuery = priceDes == "max" ? productQuery.OrderByDescending(x => x.Price) : productQuery.OrderBy(x => x.Price);

        //    var productPagedList = await _unitOfWork.GetRepository<Product>().GetPagedListAsync(
        //        predicate: null,
        //        orderBy: source => (IOrderedQueryable<Product>)productQuery,
        //        pageSize: 6,
        //        pageIndex: pageIndex
        //    );

        //    if (productPagedList == null || !productPagedList.Items.Any())
        //    {
        //        throw new NotFoundException("Not found");
        //    }

        //    var result = _mapper.Map<IEnumerable<ProductResponseForUser>>(productPagedList.Items);
        //    return result;
        //}
        public async Task UpdateAsync(string id, Product entity)
        {
            await UpdateAsync(id, entity);
        }
        public static string CreateRandomCartId()
        {
            Random ran = new();
            string CartId = "CAR";
            for (int i = 0; i < 4; i++)
            {
                CartId += ran.Next(0, 10).ToString();
            }
            return CartId;
        }
        public async Task<bool> ProductExistsAsync(int id)
        {
            var product = await _unitOfWork.GetRepository<Product>().GetFirstOrDefaultAsync(predicate: x => x.ProductId == id);
            if (product == null)
                return false;
            return true;
        }

    }
}
