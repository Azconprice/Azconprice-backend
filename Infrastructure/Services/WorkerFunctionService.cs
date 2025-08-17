using Application.Models.DTOs.Pagination;
using Application.Models.DTOs.WorkerFunction;
using Application.Models.DTOs.Profession;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using AutoMapper;
using Application.Models.DTOs.Specialization;
using Microsoft.EntityFrameworkCore;
using Application.Models.DTOs;

namespace Infrastructure.Services
{
    public class WorkerFunctionService(
        IWorkerFunctionRepository workerFunctionRepository,
        IWorkerProfileRepository workerProfileRepository,
        IWorkerFunctionSpecializationService workerFunctionSpecializationService,
        IMeasurementUnitRepository measurementUnitRepository,
        ISpecializationRepository specializationRepository,
        IProfessionRepository professionRepository,
        IWorkerService workerService,
        IMapper mapper) : IWorkerFunctionService
    {
        public async Task<WorkerFunctionShowDTO> AddSpecialization(string userId,string workerFunctionId, string specizalitionId)
        {
            var workerProfile = await workerProfileRepository.GetByUserIdAsync(userId) ?? throw new InvalidOperationException("Worker Profile not found");
            var workerFunction = await workerFunctionRepository.GetAsync(workerFunctionId) ?? throw new InvalidOperationException("Worker Function not found");
            var specialization = await specializationRepository.GetAsync(specizalitionId) ?? throw new InvalidOperationException("Specialization not found");

            if (workerFunction.ProfessionId != specialization.ProfessionId)
            {
                throw new InvalidOperationException("Specialization does not belong to the Profession of the Worker Function");
            }

            if (workerFunction.WorkerProfileId != workerProfile.Id)
            {
                throw new InvalidOperationException("Worker Function does not belong to the Worker Profile");
            }

            if (workerFunction.WorkerFunctionSpecializations.Where(wfs => wfs.SpecializationId == specialization.Id).Any())
            {
                throw new InvalidOperationException("Specialization already exists in the Worker Function");
            }

            var workerFunctionSpecialization = new WorkerFunctionSpecialization
            {
                WorkerFunctionId = workerFunction.Id,
                SpecializationId = specialization.Id
            };

            await workerFunctionSpecializationService.AddWorkerFunctionSpecializationAsync(workerFunction.Id, specialization.Id);


            return new WorkerFunctionShowDTO
            {
                Id = workerFunction.Id,
                WorkerProfileId = workerFunction.WorkerProfileId,
                MeasurementUnit = mapper.Map<MeasurementUnitShowDTO>(workerFunction.MeasurementUnit),
                Profession = mapper.Map<ProfessionShowDTO>(workerFunction.Profession),
                Price = workerFunction.Price,
                Specializations = workerFunction.WorkerFunctionSpecializations.Select(wfs => mapper.Map<SpecializationShowDTO>(wfs.Specialization)),
            };
        }

        public async Task<bool> AddWorkerFunctionAsync(string userId, CreateWorkerFunctionRequest request)
        {
            var workerProfile = await workerProfileRepository.GetByUserIdAsync(userId) ?? throw new InvalidOperationException("Worker Profile not found");
            var measurementUnit = await measurementUnitRepository.GetAsync(request.MeasurementUnitId) ?? throw new InvalidOperationException("Measurement Unit not found");
            var profession = await professionRepository.GetAsync(request.ProfessionId) ?? throw new InvalidOperationException("Profession not found");

            var workerFunction = new WorkerFunction
            {
                WorkerProfileId = workerProfile.Id,
                MeasurementUnitId = measurementUnit.Id,
                ProfessionId = profession.Id,
                Price = request.Price,
            };

            await workerFunctionRepository.AddAsync(workerFunction);
            await workerFunctionRepository.SaveChangesAsync();

            await workerFunctionSpecializationService.AddRangeOfSpecializationsToWorkerFunctionAsync(workerFunction.Id, request.SpecializationIds);

            await workerFunctionRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteWorkerFunctionAsync(string userId, string id)
        {
            var workerProfile = await workerProfileRepository.GetByUserIdAsync(userId) ?? throw new InvalidOperationException("Worker Profile not found");
            var workerFunction = await workerFunctionRepository.GetAsync(id) ?? throw new InvalidOperationException("Worker Function not found");

            if (workerFunction.WorkerProfileId != workerProfile.Id)
            {
                throw new InvalidOperationException("Worker Function does not belong to the Worker Profile");
            }

            _ = await workerFunctionRepository.RemoveAsync(workerFunction.Id.ToString());
            await workerFunctionRepository.SaveChangesAsync();
            return true;
        }

        public Task<PaginatedResult<WorkerFunctionShowDTO>> GetAllWorkerFunctionsAsync(PaginationRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<WorkerFunctionShowDTO?> GetWorkerFunctionByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<WorkerFunctionShowDTO>> GetWorkerFunctionsByWorkerProfileIdAsync(string userId)
        {
            var workerProfile = await workerProfileRepository.GetByUserIdAsync(userId) ?? throw new InvalidOperationException("Worker Profile not found");
            var workerFunctions = await workerFunctionRepository.Query().Where(wf => wf.WorkerProfileId == workerProfile.Id).ToListAsync();

            var list = workerFunctions.Select(wf => new WorkerFunctionShowDTO
            {
                Id = wf.Id,
                MeasurementUnit = mapper.Map<MeasurementUnitShowDTO>(wf.MeasurementUnit),
                Profession = mapper.Map<ProfessionShowDTO>(wf.Profession),
                WorkerProfileId = wf.WorkerProfileId,
                Price = wf.Price,
                Specializations = wf.WorkerFunctionSpecializations.Select(wfs => mapper.Map<SpecializationShowDTO>(wfs.Specialization)),
            });

            return list;
        }

        public async Task<WorkerFunction> UpdateWorkerFunctionAsync(string userId, string id, CreateWorkerFunctionRequest request)
        {
            var workerProfile = await workerProfileRepository.GetByUserIdAsync(userId) ?? throw new InvalidOperationException("Worker Profile not found");
            var workerFunction = await workerFunctionRepository.GetAsync(id) ?? throw new InvalidOperationException("Worker Function not found");

            if (workerFunction.WorkerProfileId != workerProfile.Id)
            {
                throw new InvalidOperationException("Worker Function does not belong to the Worker Profile");
            }

            var measurementUnit = await measurementUnitRepository.GetAsync(request.MeasurementUnitId) ?? throw new InvalidOperationException("Measurement Unit not found");
            var profession = await professionRepository.GetAsync(request.ProfessionId) ?? throw new InvalidOperationException("Profession not found");

            if (!await workerService.AreSpecializationsValid(request.SpecializationIds))
            {
                throw new InvalidOperationException("One or more Specializations are invalid or do not exist.");
            }

            var specializations = specializationRepository.GetWhere(s => request.SpecializationIds.Contains(s.Id.ToString()));
            var workerFunctionSpecializations = workerFunction.WorkerFunctionSpecializations.ToList();
            var workerFunctionSpecializationsToRemove = workerFunctionSpecializations
                .Where(wfs => wfs.Specialization.ProfessionId != profession.Id)
                .ToList();

            foreach (var wfs in workerFunctionSpecializationsToRemove)
            {
                await workerFunctionSpecializationService.DeleteWorkerFunctionSpecializationAsync(wfs.Id);
            }

            foreach (var specialization in specializations)
            {
                if (!workerFunctionSpecializations.Any(wfs => wfs.SpecializationId == specialization.Id))
                {
                    await workerFunctionSpecializationService.AddWorkerFunctionSpecializationAsync(workerFunction.Id, specialization.Id);
                }
            }

            workerFunction.MeasurementUnitId = measurementUnit.Id;
            workerFunction.ProfessionId = profession.Id;
            workerFunction.Price = request.Price;
            workerFunctionRepository.Update(workerFunction);
            await workerFunctionRepository.SaveChangesAsync();

            return workerFunction;
        }
    }
}
