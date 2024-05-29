using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleFishClean.Application.Common.Models.Dtos.Response
{
    public class MessageResponse
    {
        public string UserName { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
