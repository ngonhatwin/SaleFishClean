using System;
using System.Collections.Generic;

namespace SaleFishClean.Domains.Entities;

public partial class InventoryRecord
{
    public int RecordId { get; set; }

    public int? InventoryId { get; set; }

    public int Quantity { get; set; }

    public string RecordType { get; set; } = null!;

    public DateTime RecordDate { get; set; }

    public virtual Inventory? Inventory { get; set; }
}
