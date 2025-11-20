using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using src.Data;
using src.DTOs.User;
using src.Extensions;
using src.Model;

namespace src.Controller;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "ADMIN")]
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserController> _logger;

    public UserController(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        ILogger<UserController> logger)
    {
        _userManager = userManager;
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [EnableQuery(MaxTop = 100, PageSize = 10)]
    public async Task<IEnumerable<UserListResponseDto>> GetUsers()
    {
        // Query OData (chỉ scalar fields)
        var usersQuery = _context.Users
            .Include(u => u.Department)
            .Select(u => new UserListODataDto
            {
                Id = u.Id,
                UserName = u.UserName!,
                FullName = u.FullName,
                DepartmentName = u.Department != null ? u.Department.Name : null
            })
            .AsQueryable();

        // Lấy kết quả EF Core + OData
        var users = await usersQuery.ToListAsync();

        // Lấy roles cho tất cả user 1 lần
        var userIds = users.Select(u => u.Id).ToList();
        var roleMap = _context.UserRoles
            .Where(ur => userIds.Contains(ur.UserId))
            .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
            .GroupBy(x => x.UserId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Name).ToList());

        // Map vào DTO trả về
        var result = users.Select(u => new UserListResponseDto
        {
            Id = u.Id,
            UserName = u.UserName,
            FullName = u.FullName,
            DepartmentName = u.DepartmentName,
            Roles = roleMap.ContainsKey(u.Id) ? roleMap[u.Id] : new List<string>()
        });

        return result;
    }



    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetUserById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "User không tồn tại" });

        var response = await user.ToUserResponseAsync(_userManager, _context);
        return Ok(response);
    }

    [HttpGet("me")]
    [Authorize] 
    public async Task<ActionResult<UserResponse>> GetCurrentUser()
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
            return Unauthorized(new { message = "Không tìm thấy thông tin user" });

        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
            return NotFound(new { message = "User không tồn tại" });

        var response = await user.ToUserResponseAsync(_userManager, _context);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateUser([FromBody] CreateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, errorMessage, user) = await _userManager.CreateUserAsync(request, _context);

        if (!success || user == null)
            return BadRequest(new { message = errorMessage });

        var response = await user.ToUserResponseAsync(_userManager, _context);
        _logger.LogInformation("User {UserName} được tạo thành công bởi {Admin}",
            user.UserName, User.Identity?.Name);

        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserResponse>> UpdateUser(string id, [FromBody] UpdateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "User không tồn tại" });

        var (success, errorMessage) = await _userManager.UpdateUserAsync(user, request, _context);

        if (!success)
            return BadRequest(new { message = errorMessage });

        var response = await user.ToUserResponseAsync(_userManager, _context);
        _logger.LogInformation("User {UserName} được cập nhật bởi {Admin}",
            user.UserName, User.Identity?.Name);

        return Ok(response);
    }

    /// <summary>
    /// Xóa user
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User không tồn tại" });

            // Không cho phép xóa chính mình
            var currentUsername = User.Identity?.Name;
            if (user.UserName == currentUsername)
                return BadRequest(new { message = "Không thể xóa chính bạn" });

            var (success, errorMessage) = await _userManager.DeleteUserAsync(user);

            if (!success)
                return BadRequest(new { message = errorMessage });

            _logger.LogInformation("User {UserName} đã bị xóa bởi {Admin}",
                user.UserName, User.Identity?.Name);

            return Ok(new { message = "Xóa user thành công" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xóa user {UserId}", id);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa user" });
        }
    }

    /// <summary>
    /// Khóa/Mở khóa user
    /// </summary>
    [HttpPost("{id}/lockout")]
    public async Task<IActionResult> SetLockout(string id, [FromBody] bool lockout)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User không tồn tại" });

            // Không cho phép khóa chính mình
            var currentUsername = User.Identity?.Name;
            if (user.UserName == currentUsername)
                return BadRequest(new { message = "Không thể khóa chính bạn" });

            var (success, errorMessage) = await _userManager.SetLockoutAsync(user, lockout);

            if (!success)
                return BadRequest(new { message = errorMessage });

            var action = lockout ? "khóa" : "mở khóa";
            _logger.LogInformation("User {UserName} đã bị {Action} bởi {Admin}",
                user.UserName, action, User.Identity?.Name);

            return Ok(new { message = $"{(lockout ? "Khóa" : "Mở khóa")} user thành công" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi khóa/mở khóa user {UserId}", id);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi khóa/mở khóa user" });
        }
    }

    /// <summary>
    /// Reset password cho user (bởi Admin)
    /// </summary>
    [HttpPost("{id}/reset-password")]
    public async Task<IActionResult> ResetPassword(string id, [FromBody] ResetPasswordRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User không tồn tại" });

            var (success, errorMessage) = await _userManager.ResetPasswordByAdminAsync(user, request.NewPassword);

            if (!success)
                return BadRequest(new { message = errorMessage });

            _logger.LogInformation("Password của user {UserName} đã được reset bởi {Admin}",
                user.UserName, User.Identity?.Name);

            return Ok(new { message = "Reset password thành công" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi reset password cho user {UserId}", id);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi reset password" });
        }
    }


    [HttpPost("change-password")]
    [Authorize] // Bất kỳ user đã đăng nhập nào cũng có thể đổi password của mình
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { message = "Không tìm thấy thông tin user" });

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound(new { message = "User không tồn tại" });

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
                return BadRequest(new { message = string.Join(", ", result.Errors.Select(e => e.Description)) });

            _logger.LogInformation("User {UserName} đã đổi password thành công", user.UserName);

            return Ok(new { message = "Đổi password thành công" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đổi password");
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi đổi password" });
        }
    }

    /// <summary>
    /// Lấy danh sách roles của user
    /// </summary>
    [HttpGet("{id}/roles")]
    public async Task<ActionResult<List<string>>> GetUserRoles(string id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User không tồn tại" });

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy roles của user {UserId}", id);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy roles" });
        }
    }
}