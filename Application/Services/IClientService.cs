using Application.Models.DTOs.User;

namespace Application.Services
{
    public interface IClientService
    {
        Task<UserShowDTO?> GetUserByIdAsync(string id);
        Task<bool> UpdateUserAsync(string id,UserUpdateDTO model);
        Task<bool> DeleteUserAsync(string id);
    }
}
