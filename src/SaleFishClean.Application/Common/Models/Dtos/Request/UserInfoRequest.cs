using AutoMapper;
using Microsoft.AspNetCore.Http;
using SaleFishClean.Application.Common.Mappings;
using SaleFishClean.Domains.Entities;

namespace SaleFishClean.Application.Common.Models.Dtos.Request
{
    public class UserInfoRequest : IMapFrom<User>
    {
        public string UserId { get; set; }
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
        public string? Cccd { get; set; }
        public string? ImageName { get; set; }
        public IFormFile? ImageFile { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, UserInfoRequest>().ReverseMap();
        }
    }
}
