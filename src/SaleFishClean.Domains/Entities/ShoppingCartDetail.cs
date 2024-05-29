using System;
using System.Collections.Generic;

namespace SaleFishClean.Domains.Entities;

public partial class ShoppingCartDetail
{
    public string CartId { get; set; } = null!;

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual ShoppingCart Cart { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
