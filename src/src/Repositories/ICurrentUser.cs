using src.Model;

namespace src.Repositories
{
    public interface ICurrentUser
    {
        ApplicationUser? GetCurrentUser();
        long GetCurrentUserId();
    }
}