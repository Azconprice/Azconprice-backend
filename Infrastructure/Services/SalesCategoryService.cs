using Application.Models.DTOs.SalesCategory;
using Application.Repositories;
using Application.Services;
using AutoMapper;
using Domain.Entities;

namespace Infrastructure.Services
{
    public class SalesCategoryService(
        ISalesCategoryRepository salesCategoryRepository,
        IMapper mapper) : ISalesCategoryService
    {
        private readonly ISalesCategoryRepository _salesCategoryRepository = salesCategoryRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<SalesCategoryShowDTO> CreateAsync(CreateSalesCategoryDTO createSalesCategoryDTO)
        {
            // Check for uniqueness
            var existing = await _salesCategoryRepository
                .FirstOrDefaultAsync(sc => sc.Name == createSalesCategoryDTO.Name);
            if (existing != null)
                throw new InvalidOperationException("A sales category with this name already exists.");

            var entity = new SalesCategory
            {
                Name = createSalesCategoryDTO.Name
            };

            await _salesCategoryRepository.AddAsync(entity);
            await _salesCategoryRepository.SaveChangesAsync();

            return _mapper.Map<SalesCategoryShowDTO>(entity);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var entity = await _salesCategoryRepository.GetAsync(id);
            if (entity == null)
                return false;

            _salesCategoryRepository.Remove(entity);
            await _salesCategoryRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SalesCategoryShowDTO>> GetAllAsync()
        {
            var entities = await _salesCategoryRepository.GetAllAsync();
            return entities.Select(_mapper.Map<SalesCategoryShowDTO>);
        }

        public async Task<SalesCategoryShowDTO?> GetByIdAsync(string id)
        {

            var entity = await _salesCategoryRepository.GetAsync(id);
            return entity == null ? null : _mapper.Map<SalesCategoryShowDTO>(entity);
        }

        public async Task<SalesCategoryShowDTO?> UpdateAsync(string id, UpdateSalesCategoryDTO updateSalesCategoryDTO)
        {

            var entity = await _salesCategoryRepository.GetAsync(id);
            if (entity is null)
                return null;

            // Check for uniqueness
            var existing = await _salesCategoryRepository
                .FirstOrDefaultAsync(sc => sc.Name == updateSalesCategoryDTO.Name && sc.Id.ToString() != id);
            if (existing != null)
                throw new InvalidOperationException("A sales category with this name already exists.");

            entity.Name = updateSalesCategoryDTO.Name;
            _salesCategoryRepository.Update(entity);
            await _salesCategoryRepository.SaveChangesAsync();

            return _mapper.Map<SalesCategoryShowDTO>(entity);
        }
    }
}
