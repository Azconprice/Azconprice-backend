using Application.Models.DTOs.SalesCategory;
using Application.Models.DTOs.User;
using Microsoft.AspNetCore.Http;

namespace Application.Models.DTOs.Company
{
    public class CompanyProfileDTO
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public UserShowDTO User { get; set; }
        public string TaxId { get; set; }
        public string Address { get; set; }
        public string? CompanyLogo { get; set; }
        public bool IsConfirmed { get; set; }
        public string SalesCategoryId { get; set; }
        public SalesCategoryShowDTO SalesCategory { get; set; }
        public string CompanyName { get; set; }
    }
}
