using Microsoft.AspNetCore.Http;

namespace Application.Models.DTOs.Worker
{
    public class WorkerUpdateProfileDTO
    {
        // User fields
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public IEnumerable<string>? Specizalizations { get; set; }

        // WorkerProfile fields
        public bool? HaveTaxId { get; set; }
        public string? TaxId { get; set; }
        public string? Address { get; set; }
        public int? Experience { get; set; }
        public double? Price { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }
}
