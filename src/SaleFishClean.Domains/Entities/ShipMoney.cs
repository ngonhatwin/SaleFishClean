using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleFishClean.Domains.Entities
{
    public class ShipMoney
    {
        public string ShippingId { get; set; }
        public decimal? ShippingUnitPrice { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
