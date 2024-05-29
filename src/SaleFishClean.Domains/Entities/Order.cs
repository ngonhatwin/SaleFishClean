using SaleFishClean.Domains.Entities;
using System;
using System.Collections.Generic;

namespace SaleFishClean.Domains.Entities;

public partial class Order
{
    public string OrderId { get; set; } = null!;

    public string? UserId { get; set; }
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }
    public string? Email { get; set; }

    public int? PromotionId { get; set; }

    public DateTime OrderDate { get; set; }

    public string? PaymentMethod { get; set; }

    public int? Status { get; set; }

    public virtual Promotion? Promotion { get; set; }

    public virtual User? User { get; set; }
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public virtual ICollection<Shipper> Shippers { get; set; } = new List<Shipper>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
