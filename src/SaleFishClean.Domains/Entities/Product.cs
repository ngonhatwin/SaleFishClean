using Microsoft.AspNetCore.Http;
using SaleFishClean.Domains.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaleFishClean.Domains.Entities;

public partial class Product
{
    public int ProductId { get; set; }

    public int? BrandId { get; set; }

    public int? ProductTypeId { get; set; }

    public int? DiscountId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? Description { get; set; }
    
    public decimal Price { get; set; }

    public decimal Weight { get; set; }

    public string? Manufacturer { get; set; }

    public string? Color { get; set; }

    public string? Unit { get; set; }

    public bool? HasSpecialFeatures { get; set; }
    [NotMapped]
    public int? Quantity { get; set; }

    public bool? IsNew { get; set; }

    public bool? IsBestseller { get; set; }

    public bool? IsOnSale { get; set; }

    public string? SpecialNote { get; set; }

    public string? ProductImage { get; set; }
    [NotMapped]
    public IFormFile? ImageFile { get; set; }

    public int? SupplierId { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Discount? Discount { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ProductType? ProductType { get; set; }

    public virtual ICollection<ShoppingCartDetail> ShoppingCartDetails { get; set; } = new List<ShoppingCartDetail>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Supplier> SuppliersSuppliers { get; set; } = new List<Supplier>();
}
