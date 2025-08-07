using Application.Models.DTOs;
using Application.Repositories;
using Application.Services;
using Domain.Entities;

namespace Infrastructure.Services
{
    public class MeasurementUnitService(IMeasurementUnitRepository repository) : IMeasurementUnitService
    {
        private readonly IMeasurementUnitRepository _repository = repository;
        public async Task<bool> AddMeasurementUnitAsync(CreateMeasurementUnitRequest request)
        {
            var exists = _repository
                .GetWhere(m => m.Unit.ToLower() == request.Unit)
                .Any();

            if (exists)
                throw new InvalidOperationException("A Measurement Unit with this name already exists");

            var newMeasurementUnit = new MeasurementUnit()
            {
                CreatedTime = DateTime.UtcNow,
                Unit = request.Unit
            };

            await _repository.AddAsync(newMeasurementUnit);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMeasurementUnitAsync(string id)
        {
            _ = await (_repository.GetAsync(id) ?? throw new InvalidOperationException("Measurement Unit not found or could not be deleted."));

            await _repository.RemoveAsync(id);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<MeasurementUnit>> GetAllSync()
        {
            return await _repository.GetAllAsync(false);
        }

        public async Task<MeasurementUnit?> UpdateMeasurementUnitAsync(string id, CreateMeasurementUnitRequest request)
        {
            var measurementUnit = await _repository.GetAsync(id) ?? throw new InvalidOperationException("Measurement Unit not found.");
            var exists = _repository
                .GetWhere(m => m.Unit.ToLower() == request.Unit)
                .Any();

            if (exists)
                throw new InvalidOperationException("A Measurement Unit with this name already exists");

            measurementUnit.Unit = request.Unit;

            _repository.Update(measurementUnit);
            await _repository.SaveChangesAsync();

            return measurementUnit;
        }
    }
}
