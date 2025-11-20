using Microsoft.AspNetCore.Identity;

namespace src.Model;

public class ApplicationUser : IdentityUser<long>
{
    public string FullName { get; set; } = string.Empty;
    public long? DepartmentId { get; set; }

    public Department? Department { get; set; }
    public ICollection<Booking>? Bookings { get; set; }
    
}
