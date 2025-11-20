﻿using System.ComponentModel.DataAnnotations;

namespace src.DTOs.User;

public class CreateUserRequest
{
    [Required(ErrorMessage = "UserName là bắt buộc")]
    [StringLength(50, ErrorMessage = "UserName không được vượt quá 50 ký tự")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password là bắt buộc")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password phải có ít nhất 6 ký tự")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "FullName là bắt buộc")]
    [StringLength(100, ErrorMessage = "FullName không được vượt quá 100 ký tự")]
    public string FullName { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }
    
    public long? DepartmentId { get; set; }

    public List<string>? Roles { get; set; }
}

public class UpdateUserRequest
{
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string? Email { get; set; }

    [StringLength(100, ErrorMessage = "FullName không được vượt quá 100 ký tự")]
    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }
    
    public long? DepartmentId { get; set; }

    public List<string>? Roles { get; set; }
}

public class ChangePasswordRequest
{
    [Required(ErrorMessage = "Current password là bắt buộc")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "New password là bắt buộc")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "New password phải có ít nhất 6 ký tự")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm password là bắt buộc")]
    [Compare("NewPassword", ErrorMessage = "Password và Confirm password không khớp")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ResetPasswordRequest
{
    [Required(ErrorMessage = "New password là bắt buộc")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "New password phải có ít nhất 6 ký tự")]
    public string NewPassword { get; set; } = string.Empty;
}