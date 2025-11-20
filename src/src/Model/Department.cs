using System.ComponentModel.DataAnnotations;

namespace src.Model;

public class Department : BaseEntity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    // Navigation
    public ICollection<ApplicationUser>? Users { get; set; }
    public ICollection<Resource>? Resources { get; set; }
}