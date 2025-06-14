using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? RefreshToken { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    }
}
