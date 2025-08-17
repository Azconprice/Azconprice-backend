using Application.Models.DTOs.User;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs
{
    public class TransactionShowDTO
    {
        public double Amount { get; set; }
        public string OrderId { get; set; }
        public string? UserId { get; set; }
        public UserShowDTO? User { get; set; }
        public string MaskedPan { get; set; }
        public string ApprovalCode { get; set; }
        public string PaymentUrl { get; set; }
        public string SeesionId { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }
        public Guid ExcelUsagePackageId { get; set; }
        public virtual ExcelUsagePackage ExcelUsagePackage { get; set; }
    }
}
