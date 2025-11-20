using System.ComponentModel.DataAnnotations;
using src.Enum;

namespace src.Model;

public class Booking : BaseEntity
{
    [Required]
    public long ResourceId { get; set; } 

    [Required]
    public long UserId { get; set; } 

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    public string? Notes { get; set; }

    // Navigation
    public Resource? Resource { get; set; }
    public ApplicationUser? User { get; set; }
    public ICollection<BookingComment>? Comments { get; set; }
}