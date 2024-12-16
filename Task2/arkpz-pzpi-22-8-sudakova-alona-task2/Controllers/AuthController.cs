using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using FocusLearn.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Facebook;
using FocusLearn.Repositories.Abstract;

namespace FocusLearn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Авторизація за допомогою Google
        /// </summary>
        [HttpGet("login-google")]
        public IActionResult LoginGoogle()
        {
            var redirectUrl = Url.Action(nameof(GoogleResponse), "Auth");
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Principal == null)
                return Unauthorized("Google authentication failed.");

            var token = await _authService.AuthenticateGoogleUserAsync(result.Principal);
            return Ok(new { message = "Logged in successfully.", token });
        }

        /// <summary>
        /// Авторизація за допомогою Facebook
        /// </summary>
        [HttpGet("login-facebook")]
        public IActionResult LoginFacebook()
        {
            var redirectUrl = Url.Action(nameof(FacebookResponse), "Auth");
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, FacebookDefaults.AuthenticationScheme);
        }

        [HttpGet("facebook-response")]
        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Principal == null)
                return Unauthorized("Authentication failed.");

            var token = await _authService.AuthenticateFacebookUserAsync(result.Principal);
            return Ok(new { message = "Logged in successfully.", token });
        }
    }
}
