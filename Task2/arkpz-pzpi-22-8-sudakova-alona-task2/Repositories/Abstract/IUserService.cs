using FocusLearn.Models.Domain;
using FocusLearn.Models.DTO;

namespace FocusLearn.Repositories.Abstract
{
    public interface IUserService
    {
        Task AddUserAsync(User user);
        Task<User?> GetUserByProviderAsync(string providerId, string authProvider);
        Task<User?> GetUserByEmailAsync(string email);
        Task<UserDTO?> GetUserByIdAsync(int userId);
        Task<bool> UpdateUserAsync(int userId, UpdateProfileDTO updateUserDto);
        Task<bool> DeleteUserAsync(int userId);
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<IEnumerable<UserDTO>> GetAllTutorsAsync();
    }
}
