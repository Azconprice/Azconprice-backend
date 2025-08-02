using Domain.Enums;

namespace Domain.Entities
{
    public class ExcelFileRecord : BaseEntity
    {
        public string FilePath { get; set; } 
        public string FileName { get; set; } 
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public DateTime UploadedAt { get; set; }
        public ExcelFileStatus Status { get; set; }
        public string? ReviewedBy { get; set; }
        public string? Notes { get; set; }
    }
}
