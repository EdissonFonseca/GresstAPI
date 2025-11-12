using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gresst.Infrastructure.Data.Entities;

/// <summary>
/// Refresh token for JWT authentication
/// Stored in memory or database for token refresh mechanism
/// </summary>
[Table("RefreshToken")]
public class RefreshToken
{
    [Key]
    public int Id { get; set; }
    
    public long IdUsuario { get; set; }
    
    [Required]
    [StringLength(500)]
    public string Token { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? JwtId { get; set; }
    
    public bool IsUsed { get; set; }
    
    public bool IsRevoked { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public DateTime ExpiryDate { get; set; }
    
    [ForeignKey("IdUsuario")]
    public virtual Usuario? Usuario { get; set; }
}

