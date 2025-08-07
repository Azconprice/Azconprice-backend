namespace Application.Models.DTOs.WorkerFunction
{
    public class CreateWorkerFunctionRequest
    {
        public Guid WorkerProfileId { get; set; }
        public Guid ProfessionId { get; set; }
        public Guid MeasurementUnitId { get; set; }
        public decimal Price { get; set; }
        public IEnumerable<Guid> SpecializationIds { get; set; }
    }
}
