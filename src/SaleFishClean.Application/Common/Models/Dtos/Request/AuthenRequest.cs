using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleFishClean.Application.Common.Models.Dtos.Request
{
    public class AuthenRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
