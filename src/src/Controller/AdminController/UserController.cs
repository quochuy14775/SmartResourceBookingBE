using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using src.DTOs.User;
using src.Extensions;
using src.Model;
using src.Repositories;

namespace src.Controller;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "ADMIN")]
public class UserController : BaseController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository<ApplicationRole> _roleRepository;
    private readonly ILogger<UserController> _logger;

    public UserController(
        ICurrentUser currentUser,
        UserManager<ApplicationUser> userManager,
        IRepository<ApplicationRole> roleRepository,
        ILogger<UserController> logger) : base(currentUser)
    {
        _userManager = userManager;
        _roleRepository = roleRepository;
        _logger = logger;
    }

    [HttpGet]
    [EnableQuery(MaxTop = 100, PageSize = 10)]
    public async Task<IEnumerable<UserListResponseDto>> GetUsers()
    {
        var usersQuery = _userManager.Users
            .Include(u => u.Department)
            .Select(u => new UserListODataDto
            {
                Id = u.Id,
                UserName = u.UserName!,
                FullName = u.FullName,
                DepartmentName = u.Department != null ? u.Department.Name : null
            })
            .AsQueryable();

        var users = await usersQuery.ToListAsync();

        // Lấy roles từ repository
        var roles = await _roleRepository.Query()
            .Include(r => r.UserRoles)
            .ToListAsync();

        var roleMap = roles
            .SelectMany(r => r.UserRoles.Select(ur => new { ur.UserId, r.Name }))
            .GroupBy(x => x.UserId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Name).ToList());

        return users.Select(u => new UserListResponseDto
        {
            Id = u.Id,
            UserName = u.UserName,
            FullName = u.FullName,
            DepartmentName = u.DepartmentName,
            Roles = roleMap.ContainsKey(u.Id) ? roleMap[u.Id] : new List<string>()
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetUserById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "User không tồn tại" });

        var roles = await GetRolesForUserAsync(user.Id);
        var response = await user.ToUserResponseAsync(_userManager, roles);
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

        var roles = await GetRolesForUserAsync(user.Id);
        var response = await user.ToUserResponseAsync(_userManager, roles);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateUser([FromBody] CreateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, errorMessage, user) = await _userManager.CreateUserAsync(request);

        if (!success || user == null)
            return BadRequest(new { message = errorMessage });

        var roles = await GetRolesForUserAsync(user.Id);
        var response = await user.ToUserResponseAsync(_userManager, roles);
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

        var (success, errorMessage) = await _userManager.UpdateUserAsync(user, request);

        if (!success)
            return BadRequest(new { message = errorMessage });

        var roles = await GetRolesForUserAsync(user.Id);
        var response = await user.ToUserResponseAsync(_userManager, roles);
        _logger.LogInformation("User {UserName} được cập nhật bởi {Admin}",
            user.UserName, User.Identity?.Name);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "User không tồn tại" });

        if (user.UserName == User.Identity?.Name)
            return BadRequest(new { message = "Không thể xóa chính bạn" });

        var (success, errorMessage) = await _userManager.DeleteUserAsync(user);

        if (!success)
            return BadRequest(new { message = errorMessage });

        _logger.LogInformation("User {UserName} đã bị xóa bởi {Admin}",
            user.UserName, User.Identity?.Name);

        return Ok(new { message = "Xóa user thành công" });
    }

    [HttpPost("{id}/lockout")]
    public async Task<IActionResult> SetLockout(string id, [FromBody] bool lockout)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "User không tồn tại" });

        if (user.UserName == User.Identity?.Name)
            return BadRequest(new { message = "Không thể khóa chính bạn" });

        var (success, errorMessage) = await _userManager.SetLockoutAsync(user, lockout);

        if (!success)
            return BadRequest(new { message = errorMessage });

        var action = lockout ? "khóa" : "mở khóa";
        _logger.LogInformation("User {UserName} đã bị {Action} bởi {Admin}",
            user.UserName, action, User.Identity?.Name);

        return Ok(new { message = $"{(lockout ? "Khóa" : "Mở khóa")} user thành công" });
    }

    [HttpPost("{id}/reset-password")]
    public async Task<IActionResult> ResetPassword(string id, [FromBody] ResetPasswordRequest request)
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

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
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

    [HttpGet("{id}/roles")]
    public async Task<ActionResult<List<string>>> GetUserRoles(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "User không tồn tại" });

        var roles = await GetRolesForUserAsync(user.Id);
        return Ok(roles);
    }

    // ---------- Helper method ----------
    private async Task<List<string>> GetRolesForUserAsync(long userId)
    {
        var roles = await _roleRepository.Query()
            .Include(r => r.UserRoles)
            .ToListAsync();

        return roles
            .Where(r => r.UserRoles.Any(ur => ur.UserId == userId))
            .Select(r => r.Name)
            .ToList();
    }
}
