using System;
using System.Collections.Generic;

namespace SaleFishClean.Domains.Entities;


public partial class Discount
{
    public int DiscountId { get; set; }

    public string? Code { get; set; }

    public decimal? DiscountValue { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
