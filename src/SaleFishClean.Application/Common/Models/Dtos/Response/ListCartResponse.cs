

namespace SaleFishClean.Application.Common.Models.Dtos.Response
{
    public class ListCartResponse
    {
        public IEnumerable<ProductResponseForUser> Products { get; set; } = new List<ProductResponseForUser>();
    }
}
