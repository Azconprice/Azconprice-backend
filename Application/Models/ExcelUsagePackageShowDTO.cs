namespace Application.Models
{
    public class ExcelUsagePackageShowDTO
    {
        public int RowsCount { get; set; }
        public int UsedRowsCount { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string UserId { get; set; }
        public Guid TransactionId { get; set; }
    }
}
