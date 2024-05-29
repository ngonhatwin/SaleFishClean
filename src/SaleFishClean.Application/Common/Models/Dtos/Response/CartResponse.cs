using AutoMapper;
using SaleFishClean.Application.Common.Mappings;
using SaleFishClean.Domains.Entities;


namespace SaleFishClean.Application.Common.Models.Dtos.Response
{
    public class CartResponse : IMapFrom<ShoppingCartDetail>
    {
        public string? UserId { get; set; }
        public int Id { get; set; }
        public string NameProduct { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<ShoppingCartDetail, CartResponse>().ReverseMap();
        }
    }
}
