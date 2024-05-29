using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleFishClean.Application.Common.Models.Dtos.Response
{
    public class AuthenResponse
    {
        public string Id { get; set; }
        public string? UserName { get; set; }
        public bool? Access { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? Expired { get; set; }
    }
}
