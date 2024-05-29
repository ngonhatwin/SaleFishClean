using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleFishClean.Application.Common.Models.Dtos.Request
{
    public class UserConnectionRequest
    {
        public string userId { get; set; }
        public string userName { get; set; }
        public string ConnectionId { get; set; }
    }
}
