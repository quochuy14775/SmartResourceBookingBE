using System.ComponentModel.DataAnnotations;

namespace src.Model;

public class ResourceCategory : BaseEntity
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation
    public ICollection<Resource>? Resources { get; set; }
}