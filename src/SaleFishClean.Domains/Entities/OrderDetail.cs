
using SaleFishClean.Domains.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleFishClean.Domains.Entities
{
    public class OrderDetail
    {
        public string OrderId { get; set; }

        public int ProductId { get; set; }

        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public virtual Order Order { get; set; } = null!;

        public virtual Product Product { get; set; } = null!;
    }
}
