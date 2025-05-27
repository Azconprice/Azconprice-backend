using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? RefreshToken { get; set; }
        public virtual ICollection<UserSpecialization> UserSpecializations { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
