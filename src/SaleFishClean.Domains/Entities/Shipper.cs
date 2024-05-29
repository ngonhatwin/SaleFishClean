
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleFishClean.Domains.Entities
{
    public class Shipper
    {
        public int Id { get; set; }
        public string ShipperId { get; set; }
        public string OrderID { get; set; }
        public DateTime? CreatedAt { get; set; }
        public virtual Order Order { get; set; }
        public virtual User User { get; set; }
    }
}