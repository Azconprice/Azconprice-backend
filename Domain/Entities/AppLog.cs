namespace Domain.Entities
{
    public class AppLog : BaseEntity
    {
        public string Action { get; set; } = null!;
        public string? RelatedEntityId { get; set; } 
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Details { get; set; }
    }
}