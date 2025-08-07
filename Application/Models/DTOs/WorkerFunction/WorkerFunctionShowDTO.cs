using Application.Models.DTOs.Profession;
using Application.Models.DTOs.Specialization;
using Domain.Entities;

namespace Application.Models.DTOs.WorkerFunction
{
    public class WorkerFunctionShowDTO
    {
        public ProfessionShowDTO Profession { get; set; }
        public MeasurementUnit MeasurementUnit { get; set; }
        public Guid WorkerProfileId { get; set; }
        public decimal Price { get; set; }
        public IEnumerable<SpecializationShowDTO> Specializations { get; set; }
    }
}
