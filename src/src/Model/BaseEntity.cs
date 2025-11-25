using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Model;

public abstract class BaseEntity
{
    [Key]
    public long Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
}