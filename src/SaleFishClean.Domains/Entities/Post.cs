using System;
using System.Collections.Generic;

namespace SaleFishClean.Domains.Entities;

public partial class Post
{
    public int PostId { get; set; }

    public string? UserId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public DateTime? PostedDate { get; set; }

    public virtual User? User { get; set; }
}
