using Application.Models.DTOs.Profession;
using Application.Models.DTOs.Specialization;
using Domain.Entities;

namespace Application.Models.DTOs.WorkerFunction
{
    public class WorkerFunctionShowDTO
    {
        public Guid Id { get; set; }
        public ProfessionShowDTO Profession { get; set; }
        public MeasurementUnitShowDTO MeasurementUnit { get; set; }
        public Guid WorkerProfileId { get; set; }
        public double Price { get; set; }
        public IEnumerable<SpecializationShowDTO> Specializations { get; set; }
    }
}
