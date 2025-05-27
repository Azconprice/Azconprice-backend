using Microsoft.AspNetCore.Http;

namespace Application.Models.DTOs.Worker
{
    public class WorkerUpdateProfileDTO
    {
        public bool? HaveTaxId { get; set; }
        public string? TaxId { get; set; }
        public string? Address { get; set; }
        public int? Experience { get; set; }
        public double? Price { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }
}
