namespace Application.Models.DTOs.WorkerFunction
{
    public class CreateWorkerFunctionRequest
    {
        public string ProfessionId { get; set; }
        public string MeasurementUnitId { get; set; }
        public double Price { get; set; }
        public IEnumerable<string> SpecializationIds { get; set; }
    }
}
