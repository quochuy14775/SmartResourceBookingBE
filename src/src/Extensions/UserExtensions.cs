using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using src.Data;
using src.DTOs.User;
using src.Model;

namespace src.Extensions;

public static class UserExtensions
{
    /// <summary>
    /// Tạo user mới
    /// </summary>
    public static async Task<(bool Success, string? ErrorMessage, ApplicationUser? User)> CreateUserAsync(
        this UserManager<ApplicationUser> userManager,
        CreateUserRequest request,
        ApplicationDbContext context)
    {
        // Kiểm tra username đã tồn tại
        var existingUser = await userManager.FindByNameAsync(request.UserName);
        if (existingUser != null)
            return (false, "Username đã tồn tại", null);

        // Kiểm tra email đã tồn tại
        var existingEmail = await userManager.FindByEmailAsync(request.Email);
        if (existingEmail != null)
            return (false, "Email đã được sử dụng", null);

        // // Kiểm tra department nếu có
        // if (!string.IsNullOrEmpty(request.DepartmentId))
        // {
        //     var department = await context.Departments.FindAsync(request.DepartmentId);
        //     if (department == null)
        //         return (false, "Department không tồn tại", null);
        // }

        // Tạo user mới
        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            DepartmentId = request.DepartmentId,
            EmailConfirmed = true // Mặc định confirm email
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)), null);

        // Gán roles nếu có
        if (request.Roles != null && request.Roles.Any())
        {
            var roleResult = await userManager.AddToRolesAsync(user, request.Roles);
            if (!roleResult.Succeeded)
            {
                // Rollback: xóa user nếu gán role thất bại
                await userManager.DeleteAsync(user);
                return (false, "Không thể gán roles cho user", null);
            }
        }

        return (true, null, user);
    }

    /// <summary>
    /// Cập nhật thông tin user
    /// </summary>
    public static async Task<(bool Success, string? ErrorMessage)> UpdateUserAsync(
        this UserManager<ApplicationUser> userManager,
        ApplicationUser user,
        UpdateUserRequest request,
        ApplicationDbContext context)
    {
        // Cập nhật email nếu có
        if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
        {
            var existingEmail = await userManager.FindByEmailAsync(request.Email);
            if (existingEmail != null && existingEmail.Id != user.Id)
                return (false, "Email đã được sử dụng bởi user khác");

            user.Email = request.Email;
        }

        // Cập nhật các thông tin khác
        if (!string.IsNullOrEmpty(request.FullName))
            user.FullName = request.FullName;

        if (request.PhoneNumber != null)
            user.PhoneNumber = request.PhoneNumber;

        // Cập nhật department
        if (request.DepartmentId != null)
        {
            if (request.DepartmentId.HasValue)
            {
                var department = await context.Departments.FindAsync(request.DepartmentId.Value);
                if (department == null)
                    return (false, "Department không tồn tại");

                user.DepartmentId = request.DepartmentId;
            }
        }

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

        // Cập nhật roles nếu có
        if (request.Roles != null)
        {
            var currentRoles = await userManager.GetRolesAsync(user);
            var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
                return (false, "Không thể cập nhật roles");

            if (request.Roles.Any())
            {
                var addResult = await userManager.AddToRolesAsync(user, request.Roles);
                if (!addResult.Succeeded)
                    return (false, "Không thể cập nhật roles");
            }
        }

        return (true, null);
    }

    /// <summary>
    /// Xóa user
    /// </summary>
    public static async Task<(bool Success, string? ErrorMessage)> DeleteUserAsync(
        this UserManager<ApplicationUser> userManager,
        ApplicationUser user)
    {
        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

        return (true, null);
    }

    /// <summary>
    /// Lấy thông tin chi tiết user
    /// </summary>
    public static async Task<UserResponse?> ToUserResponseAsync(
        this ApplicationUser user,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
        var roles = await userManager.GetRolesAsync(user);
        var department = user.DepartmentId != null
            ? await context.Departments.FindAsync(user.DepartmentId)
            : null;

        return new UserResponse
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            DepartmentId = user.DepartmentId,
            DepartmentName = department?.Name,
            Roles = roles.ToList(),
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            LockoutEnabled = user.LockoutEnabled,
            LockoutEnd = user.LockoutEnd,
            AccessFailedCount = user.AccessFailedCount
        };
    }

    /// <summary>
    /// Chuyển đổi sang UserListResponseDto
    /// </summary>
    public static UserListResponseDto ToUserListResponse(
        this ApplicationUser user,
        List<string> roles,
        string? departmentName)
    {
        return new UserListResponseDto
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            DepartmentName = departmentName,
            Roles = roles,
            IsLockedOut = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow
        };
    }


    /// <summary>
    /// Lock/Unlock user
    /// </summary>
    public static async Task<(bool Success, string? ErrorMessage)> SetLockoutAsync(
        this UserManager<ApplicationUser> userManager,
        ApplicationUser user,
        bool lockout)
    {
        var lockoutEnd = lockout ? DateTimeOffset.MaxValue : (DateTimeOffset?)null;
        var result = await userManager.SetLockoutEndDateAsync(user, lockoutEnd);

        if (!result.Succeeded)
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

        return (true, null);
    }

    /// <summary>
    /// Reset password bởi admin
    /// </summary>
    public static async Task<(bool Success, string? ErrorMessage)> ResetPasswordByAdminAsync(
        this UserManager<ApplicationUser> userManager,
        ApplicationUser user,
        string newPassword)
    {
        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, resetToken, newPassword);

        if (!result.Succeeded)
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

        return (true, null);
    }
}


