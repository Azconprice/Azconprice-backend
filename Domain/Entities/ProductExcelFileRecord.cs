using Domain.Enums;

namespace Domain.Entities
{
    public class ProductExcelFileRecord : BaseEntity
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string CompanyProfileId { get; set; }
        public virtual CompanyProfile CompanyProfile { get; set; }
        public DateTime UploadedAt { get; set; }
        public ExcelFileStatus Status { get; set; }
        public string? ReviewedBy { get; set; }
        public string? Notes { get; set; }
    }
}
