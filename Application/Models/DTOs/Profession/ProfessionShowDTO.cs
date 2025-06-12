using Application.Models.DTOs.Specialization;

namespace Application.Models.DTOs.Profession
{
    public class ProfessionShowDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<SpecializationInsideProfessionDTO> Specializations { get; set; }
    }
}
