using Arch.EntityFrameworkCore.UnitOfWork.Collections;
using AutoMapper;
using SaleFishClean.Application.Common.Mappings;
using SaleFishClean.Domains.Entities;


namespace SaleFishClean.Application.Common.Models.Dtos.Response
{
    public class ProductResponseForUser : IMapFrom<Product>
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? Description { get; set; }
        public string? ProductImage { get; set; }
        public decimal Price { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Product, ProductResponseForUser>().ReverseMap();
            profile.CreateMap<PagedList<Product>, IEnumerable<ProductResponseForUser>>()
            .ConvertUsing((source, destination, context) =>
                context.Mapper.Map<IEnumerable<ProductResponseForUser>>(source.Items));

        }
    }
}
