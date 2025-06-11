using Application.Models.DTOs.SalesCategory;

namespace Application.Services
{
    public interface ISalesCategoryService
    {
        Task<SalesCategoryShowDTO> CreateAsync(CreateSalesCategoryDTO createSalesCategoryDTO);
        Task<SalesCategoryShowDTO?> GetByIdAsync(string id);
        Task<IEnumerable<SalesCategoryShowDTO>> GetAllAsync();
        Task<SalesCategoryShowDTO?> UpdateAsync(string id,UpdateSalesCategoryDTO updateSalesCategoryDTO);
        Task<bool> DeleteAsync(string id);
    }
}
