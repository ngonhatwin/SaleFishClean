using System;
using System.Collections.Generic;

namespace SaleFishClean.Domains.Entities;

public partial class RefreshToken
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string TokenRefresh { get; set; } = null!;

    public DateTime Expires { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Revoked { get; set; }

    public string? ReplaceByToken { get; set; }

    public virtual User User { get; set; } = null!;
    public bool IsExpired => DateTime.Now >= Expires;

    // check cột revoke != null => isrevoked == true
    public bool IsRevoked => Revoked != null;

    public bool IsActive => !IsRevoked && !IsExpired;
}
