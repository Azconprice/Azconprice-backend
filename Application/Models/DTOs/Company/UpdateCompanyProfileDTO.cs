using Microsoft.AspNetCore.Http;

namespace Application.Models.DTOs.Company
{
    public class UpdateCompanyProfileDTO
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? SalesCategoryId { get; set; }
        public IFormFile? Logo { get; set; }
    }
}
