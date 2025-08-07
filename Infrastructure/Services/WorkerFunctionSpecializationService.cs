using Application.Repositories;
using Application.Services;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class WorkerFunctionSpecializationService(IWorkerFunctionRepository workerFunctionRepository, ISpecializationRepository specializationRepository, IWorkerFunctionSpecializationRepository workerFunctionSpecializationRepository) : IWorkerFunctionSpecializationService
    {
        public async Task<bool> AddRangeOfSpecializationsToWorkerFunctionAsync(Guid workerFunctionId, IEnumerable<string> specializationIds)
        {
            try
            {
                var workerFunction = await workerFunctionRepository.GetAsync(workerFunctionId.ToString()) ?? throw new ArgumentException($"Worker function with ID {workerFunctionId} does not exist.");

                var WorkerFunctionSpecializations = new List<WorkerFunctionSpecialization>();

                foreach (var specializationId in specializationIds)
                {
                    WorkerFunctionSpecializations.Add(new WorkerFunctionSpecialization
                    {
                        WorkerFunctionId = workerFunction.Id,
                        SpecializationId = Guid.Parse(specializationId)
                    });
                }

                await workerFunctionSpecializationRepository.AddRangeAsync(WorkerFunctionSpecializations);
                await workerFunctionSpecializationRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> AddWorkerFunctionSpecializationAsync(Guid workerFunctionId, Guid specializationId)
        {
            var workerFunction = await workerFunctionRepository.GetAsync(workerFunctionId.ToString()) ?? throw new ArgumentException($"Worker function with ID {workerFunctionId} does not exist.");

            var specialization = await specializationRepository.GetAsync(specializationId.ToString()) ?? throw new ArgumentException($"Specialization with ID {specializationId} does not exist.");

            var workerFunctionSpecialization = new WorkerFunctionSpecialization
            {
                WorkerFunctionId = workerFunctionId,
                SpecializationId = specializationId
            };

            await workerFunctionSpecializationRepository.AddAsync(workerFunctionSpecialization);
            await workerFunctionSpecializationRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteWorkerFunctionSpecializationAsync(Guid id)
        {
            _ = await workerFunctionRepository.GetAsync(id.ToString()) ?? throw new ArgumentException($"Worker function with ID {id} does not exist.");

            await workerFunctionSpecializationRepository.RemoveAsync(id.ToString());
            await workerFunctionSpecializationRepository.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<WorkerFunctionSpecialization>> GetWorkerFunctionSpecializationsByWorkerFunctionIdAsync(Guid workerFunctionId)
        {
            var list = await workerFunctionSpecializationRepository.Query()
                .Where(wfs => wfs.WorkerFunctionId == workerFunctionId)
                .ToListAsync();

            return list;
        }
    }
}
