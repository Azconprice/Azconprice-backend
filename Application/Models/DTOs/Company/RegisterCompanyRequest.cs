using Microsoft.AspNetCore.Http;

namespace Application.Models.DTOs.Company
{
    public class RegisterCompanyRequest
    {
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string PhoneNumber { get; set; }
        public IFormFile TaxId { get; set; }
        public string Address { get; set; }
        public string SalesCategoryId { get; set; }
        public IFormFile? Logo { get; set; }
        public IFormFile ProductsExcel { get; set; }
    }
}
