using System;
using System.Collections.Generic;

namespace SaleFishClean.Domains.Entities;

public partial class ShoppingCart
{
    public string CartId { get; set; } = null!;

    public string? UserId { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public virtual ICollection<ShoppingCartDetail> ShoppingCartDetails { get; set; } = new List<ShoppingCartDetail>();

    public virtual User? User { get; set; }
}
