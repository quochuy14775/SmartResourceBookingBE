

namespace src.Model;

public class AuditTrail : BaseEntity
{
    public long? UserId { get; set; }
    public string Action { get; set; } = string.Empty; // Create / Update / Delete
    public string EntityName { get; set; } = string.Empty;
    public long? EntityId { get; set; }
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}