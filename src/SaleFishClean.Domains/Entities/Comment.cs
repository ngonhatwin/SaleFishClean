﻿using System;
using System.Collections.Generic;

namespace SaleFishClean.Domains.Entities;


public partial class Comment
{
    public int CommentId { get; set; }

    public int? ProductId { get; set; }

    public string? UserId { get; set; }

    public string? Content { get; set; }

    public DateTime? CommentDate { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
