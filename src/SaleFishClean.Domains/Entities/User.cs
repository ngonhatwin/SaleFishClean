using System;
using System.Collections.Generic;

namespace SaleFishClean.Domains.Entities;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool? IsVerify { get; set; }

    public DateTime? ResetTokenExpires { get; set; }

    public string? CodeOtp { get; set; }

    public DateTime? OtpCreate { get; set; }

    public DateTime? OtpExpired { get; set; }

    public bool? IsBlocked { get; set; }

    public DateTime? BlockedTime { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public bool? Gender { get; set; }

    public string? Cccd { get; set; }

    public string? RollName { get; set; }

    public string? ImageName { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<Shipper> Shippers { get; set; } = new List<Shipper>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
}
