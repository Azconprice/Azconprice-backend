namespace Application.Models.DTOs.AppLogs
{
    public class LogListItemDTO
    {
        public Guid Id { get; set; }
        public string Action { get; set; } = null!;
        public string? RelatedEntityId { get; set; }
        public string? UserName { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Details { get; set; }
    }
}