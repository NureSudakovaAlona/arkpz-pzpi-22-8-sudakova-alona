using FocusLearn.Models.Domain;
using System.Security.Claims;

namespace FocusLearn.Repositories.Abstract
{
    public interface IAuthService
    {
        Task<string> AuthenticateGoogleUserAsync(ClaimsPrincipal principal);
        Task<string> AuthenticateFacebookUserAsync(ClaimsPrincipal principal);
    }
}
