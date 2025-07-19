using System;
using System.ComponentModel.DataAnnotations;

namespace TastyGo.Models;

public class RefreshToken : AuditableEntity
{
    public string Token { get; set; }
    public string JwtId { get; set; }
    public Guid UserId { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool Invalidated { get; set; }
    public bool Used { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public User User { get; set; }
}
