using AutoMapper;
using SaleFishClean.Application.Common.Mappings;
using SaleFishClean.Domains.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace SaleFishClean.Application.Common.Models.Dtos.Request
{
    public class ProductRequest : IMapFrom<Product>
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;

        public string? Description { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal Price { get; set; }

        public decimal Weight { get; set; }

        public string? Unit { get; set; }

        public string? Manufacturer { get; set; }
        public string? ProductImage { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Product, ProductRequest>().ReverseMap();
        }
    }
}
