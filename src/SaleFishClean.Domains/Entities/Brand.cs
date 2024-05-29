using System;
using System.Collections.Generic;

namespace SaleFishClean.Domains.Entities;


public partial class Brand
{
    public int BrandId { get; set; }

    public string BrandName { get; set; } = null!;

    public string? CountryOfOrigin { get; set; }

    public DateOnly? FoundedDate { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
