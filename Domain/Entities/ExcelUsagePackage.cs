namespace Domain.Entities
{
    public class ExcelUsagePackage : BaseEntity
    {
        public int RowsCount { get; set; }
        public int UsedRowsCount { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }
    }
}
