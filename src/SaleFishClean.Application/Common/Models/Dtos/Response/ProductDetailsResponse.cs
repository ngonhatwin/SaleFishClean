using Arch.EntityFrameworkCore.UnitOfWork.Collections;
using AutoMapper;
using SaleFishClean.Application.Common.Mappings;
using SaleFishClean.Domains.Entities;


namespace SaleFishClean.Application.Common.Models.Dtos.Response
{
    public class ProductDetailsResponse : IMapFrom<Product>
    {
        public int ProductId { get; set; }
        public string ProductImage { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;
        public string Manufacturer { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Product, ProductDetailsResponse>().ReverseMap();
            profile.CreateMap<PagedList<Product>, IEnumerable<ProductDetailsResponse>>()
           .ConvertUsing((source, destination, context) =>
               context.Mapper.Map<IEnumerable<ProductDetailsResponse>>(source.Items));
        }
    }
}
