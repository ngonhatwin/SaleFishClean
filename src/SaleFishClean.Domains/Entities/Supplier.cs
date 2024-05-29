using System;
using System.Collections.Generic;

namespace SaleFishClean.Domains.Entities;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string SupplierName { get; set; } = null!;

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? ContactPerson { get; set; }

    public string? ContactEmail { get; set; }

    public DateTime RegistrationDate { get; set; }

    public int ProductId { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<Product> ProductsProducts { get; set; } = new List<Product>();
}
