using Application.Models.DTOs.User;

namespace Application.Services
{
    public interface IClientService
    {
        Task<UserShowDTO?> GetUserByIdAsync(string id);
        Task<bool> UpdateUserAsync(string id,UserUpdateDTO model, Func<string, string, string> generateConfirmationUrl);
        Task<bool> DeleteUserAsync(string id);
    }
}
