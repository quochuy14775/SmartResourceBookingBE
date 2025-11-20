using Microsoft.AspNetCore.Identity;

namespace src.Model;


public class ApplicationRole : IdentityRole<long>
{
    public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
}

public class ApplicationUserRole : IdentityUserRole<long> { }