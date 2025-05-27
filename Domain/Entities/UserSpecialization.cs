using System;

namespace Domain.Entities
{
    public class UserSpecialization : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid SpecializationId { get; set; }
        public Specialization Specialization { get; set; }
    }
}