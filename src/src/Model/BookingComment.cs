﻿using System.ComponentModel.DataAnnotations;

namespace src.Model;

public class BookingComment : BaseEntity
{
    [Required]
    public long BookingId { get; set; }

    [Required]
    public long UserId { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Booking? Booking { get; set; }
    public ApplicationUser? User { get; set; }
}