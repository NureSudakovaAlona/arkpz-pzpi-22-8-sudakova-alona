using FocusLearn.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

public class UserStatusFilter : IAsyncAuthorizationFilter
{
    private readonly FocusLearnDbContext _context;

    public UserStatusFilter(FocusLearnDbContext context)
    {
        _context = context;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) ??
          context.HttpContext.User.FindFirst("sub") ??
          context.HttpContext.User.FindFirst("UserId");

        if (userIdClaim == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var userId = int.Parse(userIdClaim.Value);

        var user = await _context.Users.FindAsync(userId);
        if (user == null || user.ProfileStatus != "Active")
        {
            context.Result = new ForbidResult();
        }
    }
}
