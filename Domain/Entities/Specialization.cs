using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Specialization : BaseEntity
    {
        public string Name { get; set; }

        // Many-to-one: Each Specialization belongs to one Category
        public Guid ProfessionId { get; set; }
        public Profession Profession { get; set; }

        // Many-to-many: Specialization <-> User (Worker)
        public ICollection<UserSpecialization> UserSpecializations { get; set; }
    }
}