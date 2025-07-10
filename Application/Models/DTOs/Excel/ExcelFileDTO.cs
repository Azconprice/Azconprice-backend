namespace Application.Models.DTOs.Excel
{
    public class ExcelFileDTO
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
