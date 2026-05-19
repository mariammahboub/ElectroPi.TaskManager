using ElectroPi.TaskManager.Application.Common.Exceptions;
using System.Security.Claims;

namespace ElectroPi.TaskManager.API.Extensions
{

    public static class ClaimsPrincipalExtensions
    {

        public static Guid GetUserId(this ClaimsPrincipal principal)
        {
            var claim = principal.FindFirstValue("uid")
                     ?? principal.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? throw new UnauthorizedException("User identity claim not found in token.");

            return Guid.TryParse(claim, out var userId)
                ? userId
                : throw new UnauthorizedException("User identity claim is not a valid Guid.");
        }

        public static string GetUserEmail(this ClaimsPrincipal principal)
            => principal.FindFirstValue(ClaimTypes.Email)
            ?? throw new UnauthorizedException("Email claim not found in token.");

        public static string GetUserRole(this ClaimsPrincipal principal)
            => principal.FindFirstValue(ClaimTypes.Role)
            ?? throw new UnauthorizedException("Role claim not found in token.");
    }
}