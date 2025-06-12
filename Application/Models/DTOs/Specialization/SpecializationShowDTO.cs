using Application.Models.DTOs.Profession;

namespace Application.Models.DTOs.Specialization
{
    public class SpecializationShowDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ProfessionId { get; set; }
        public ProfessionInsideSpecializationDTO Profession { get; set; }
    }
}