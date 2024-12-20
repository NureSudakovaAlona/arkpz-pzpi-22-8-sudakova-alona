using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using FocusLearn.Models.Domain;
using FocusLearn.Repositories.Abstract;

namespace FocusLearn.Repositories.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AuthService(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }
        
        /// <summary>
        /// Автентифікація з Google
        /// </summary>
        public async Task<string> AuthenticateGoogleUserAsync(ClaimsPrincipal principal)
        {
            return await AuthenticateExternalUserAsync(principal, "Google");
        }

        /// <summary>
        /// Автентифікація з Facebook
        /// </summary>
        public async Task<string> AuthenticateFacebookUserAsync(ClaimsPrincipal principal)
        {
            return await AuthenticateExternalUserAsync(principal, "Facebook");
        }

        /// <summary>
        /// Автентифікація з зовнішнім API
        /// </summary>
        private async Task<string> AuthenticateExternalUserAsync(ClaimsPrincipal principal, string provider)
        {
            var email = principal.FindFirstValue(ClaimTypes.Email);
            var name = principal.FindFirstValue(ClaimTypes.Name);
            var providerId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var photoUrl = principal.FindFirstValue("picture") ?? "default_photo_url";
            var language = principal.FindFirstValue("locale") ?? "en";

            // Перевірка, чи існує користувач із таким Email або ProviderId
            var user = await _userService.GetUserByProviderAsync(providerId, provider);

            if (user == null)
            {
                user = await _userService.GetUserByEmailAsync(email);

                if (user != null)
                {
                    user.ProviderId = providerId;
                    user.AuthProvider = provider;
                }
                else
                {
                    user = new User
                    {
                        UserName = name,
                        Email = email,
                        Role = "Student",
                        ProfilePhoto = photoUrl,
                        Language = language,
                        ProfileStatus = "Active",
                        AuthProvider = provider,
                        ProviderId = providerId,
                        RegistrationDate = DateTime.UtcNow
                    };

                    await _userService.AddUserAsync(user);
                }
            }
            return GenerateJwtToken(user);
        }

        /// <summary>
        /// Генерування токену
        /// </summary>
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()), // Більш стандартний claim
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),                
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("ProviderId", user.ProviderId),
                new Claim(ClaimTypes.Role, user.Role ?? "Student")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
