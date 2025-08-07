using Application.Models.DTOs;
using Domain.Entities;

namespace Application.Services
{
    public interface IMeasurementUnitService
    {
        Task<bool> AddMeasurementUnitAsync(CreateMeasurementUnitRequest request);
        Task<MeasurementUnit?> UpdateMeasurementUnitAsync(string id, CreateMeasurementUnitRequest request);
        Task<bool> DeleteMeasurementUnitAsync(string id);
        Task<IEnumerable<MeasurementUnit>> GetAllSync();
    }
}
