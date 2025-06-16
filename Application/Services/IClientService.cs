using Application.Models.DTOs.Pagination;
using Application.Models.DTOs.User;

namespace Application.Services
{
    public interface IClientService
    {
        Task<UserShowDTO?> GetUserByIdAsync(string id);
        Task<UserShowDTO?> UpdateUserAsync(string id,UserUpdateDTO model, Func<string, string, string> generateConfirmationUrl);
        Task<bool> DeleteUserAsync(string id);
        Task<PaginatedResult<UserShowDTO>> GetAllUsersAsync(PaginationRequest request);
    }
}
