namespace Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public double Amount { get; set; }
        public string OrderId { get; set; }
        public string? UserId { get; set; }
        public virtual User? User { get; set; }
        public string MaskedPan { get; set; }
        public string ApprovalCode { get; set; }
        public string PaymentUrl { get; set; }
        public string SeesionId { get; set; }
        public string InvoiceUUID { get; set; }
        public string? Description { get; set; }
        public long TransactionId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }
        public bool? IsReversed { get; set; }
        public bool? IsInstallment { get; set; }
        public Guid ExcelUsagePackageId { get; set; }
        public virtual ExcelUsagePackage ExcelUsagePackage { get; set; }
    }
}
