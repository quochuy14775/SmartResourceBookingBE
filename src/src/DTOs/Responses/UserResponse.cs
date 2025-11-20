﻿namespace src.DTOs.User;

public class UserResponse
{
    public long Id { get; set; } 
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public long? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public List<string> Roles { get; set; } = new();
    public bool EmailConfirmed { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool LockoutEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public int AccessFailedCount { get; set; }
}

public class UserListResponseDto
{
    public long Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? DepartmentName { get; set; }
    public List<string> Roles { get; set; } = new();
    public bool IsLockedOut { get; set; }
    
    
}
 
public class UserListODataDto
{
    public long Id { get; set; }
    public string UserName { get; set; } = null!;
    public string? FullName { get; set; }
    public string? DepartmentName { get; set; }
}

