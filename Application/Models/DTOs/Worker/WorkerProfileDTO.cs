using Application.Models.DTOs.User;

namespace Application.Models.DTOs.Worker
{
    public class WorkerProfileDTO
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public UserShowDTO User { get; set; }
        public bool HaveTaxId { get; set; }
        public string? TaxId { get; set; }
        public int Experience { get; set; }
        public double Price { get; set; }
    }
}
