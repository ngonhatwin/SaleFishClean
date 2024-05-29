using System;
using System.Collections.Generic;

namespace SaleFishClean.Domains.Entities;

public partial class Inventory
{
    public int InventoryId { get; set; }

    public int? SupplierId { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public DateTime EntryDate { get; set; }

    public virtual ICollection<InventoryRecord> InventoryRecords { get; set; } = new List<InventoryRecord>();

    public virtual Product? Product { get; set; }

    public virtual Supplier? Supplier { get; set; }
}
