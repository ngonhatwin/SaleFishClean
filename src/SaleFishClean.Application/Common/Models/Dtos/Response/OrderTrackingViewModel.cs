using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleFishClean.Application.Common.Models.Dtos.Response
{
    public class OrderTrackingViewModel
    {
        public string OrderId { get; set; }
        public string BuyerName { get; set; }
        public string ShipperName { get; set; }
        public decimal TotalAmount { get; set; }
        public int? Status { get; set; } // "Đang vận chuyển", "Đã nhận hàng", "Đã hủy hàng"
    }
}
