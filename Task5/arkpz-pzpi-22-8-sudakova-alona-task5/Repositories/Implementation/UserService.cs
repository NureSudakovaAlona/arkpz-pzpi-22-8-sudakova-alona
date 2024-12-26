using FocusLearn.Models.Domain;
using FocusLearn.Models.DTO;
using FocusLearn.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace FocusLearn.Repositories.Implementation
{
    public class UserService : IUserService
    {
        private readonly FocusLearnDbContext _context;

        public UserService(FocusLearnDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отримати всіх користувачів
        /// </summary>
        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Email = u.Email,
                    ProfilePhoto = u.ProfilePhoto,
                    Language = u.Language,
                    Role = u.Role,
                    ProfileStatus = u.ProfileStatus
                })
                .ToListAsync();
        }

        /// <summary>
        /// Отримати всіх репетиторів
        /// </summary>
        public async Task<IEnumerable<UserDTO>> GetAllTutorsAsync()
        {
            return await _context.Users
                .Where(u => u.Role == "Tutor")
                .Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Email = u.Email,
                    ProfilePhoto = u.ProfilePhoto,
                    Language = u.Language,
                    Role = u.Role,
                    ProfileStatus = u.ProfileStatus
                })
                .ToListAsync();
        }

        /// <summary>
        /// Отримати користувача за провайдером
        /// </summary>
        public async Task<User?> GetUserByProviderAsync(string providerId, string authProvider)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.ProviderId == providerId && u.AuthProvider == authProvider);
        }

        /// <summary>
        /// Отримати користувача за email
        /// </summary>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Отримати користувача за ID
        /// </summary>
        public async Task<UserDTO?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Email = u.Email,
                    ProfilePhoto = u.ProfilePhoto,
                    Language = u.Language,
                    Role = u.Role,
                    ProfileStatus = u.ProfileStatus
                })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Додати користувача
        /// </summary>
        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Редагувати користувача
        /// </summary>
        public async Task<bool> UpdateUserAsync(int userId, UpdateProfileDTO updateUserDto)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return false;

            user.UserName = updateUserDto.UserName ?? user.UserName;
            user.ProfilePhoto = updateUserDto.ProfilePhoto ?? user.ProfilePhoto;
            user.Language = updateUserDto.Language ?? user.Language;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Видалити користувача
        /// </summary>
        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

