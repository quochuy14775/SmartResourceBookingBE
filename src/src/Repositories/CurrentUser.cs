using Microsoft.AspNetCore.Identity;
using src.Model;

namespace src.Repositories
{
    public class CurrentUser : ICurrentUser
    {
        private ApplicationUser? _currentUser;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public CurrentUser(UserManager<ApplicationUser> userManager,
            IHttpContextAccessor contextAccessor)
        {
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }

        public string? Name => _contextAccessor.HttpContext?.User.Identity?.Name;

        public ApplicationUser? GetCurrentUser()
        {
            if (_currentUser != null)
            {
                return _currentUser;
            }

            var contextUser = _contextAccessor.HttpContext?.User;
            if (contextUser == null)
            {
                return null;
            }

            // Synchronously wait here is deliberate to keep the method signature simple.
            _currentUser = _userManager.GetUserAsync(contextUser).GetAwaiter().GetResult();
            return _currentUser;
        }

        public long GetCurrentUserId()
        {
            if (_currentUser != null)
            {
                return _currentUser.Id;
            }

            var contextUser = _contextAccessor.HttpContext?.User;
            if (contextUser != null)
            {
                string? id = _userManager.GetUserId(contextUser);
                if (id != null && long.TryParse(id, out var parsed))
                {
                    return parsed;
                }
            }

            return 0;
        }
    }
}