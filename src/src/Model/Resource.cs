﻿using System.ComponentModel.DataAnnotations;

namespace src.Model;

public class Resource : BaseEntity
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public long CategoryId { get; set; }

    [Required]
    public long DepartmentId { get; set; }

    public bool IsAvailable { get; set; } = true;

    // Navigation
    public ResourceCategory? Category { get; set; }
    public Department? Department { get; set; }
    public ICollection<Booking>? Bookings { get; set; }
}