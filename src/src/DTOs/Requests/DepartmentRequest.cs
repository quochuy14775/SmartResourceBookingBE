using System.ComponentModel.DataAnnotations;

namespace src.DTOs.Requests;

public class DepartmentRequest
{
    [Required] [StringLength(100)] public required string Name { get; set; }

    [StringLength(500)] public required string Description { get; set; }
    
    public bool IsActive { get; set; } = true;
}