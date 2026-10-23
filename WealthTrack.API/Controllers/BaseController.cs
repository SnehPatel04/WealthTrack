using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace WealthTrack.API.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim == null ? 0 : int.Parse(claim.Value);
        }

        protected string GetCurrentUserRole()
        {
            var claim = User.FindFirst(ClaimTypes.Role);
            return claim?.Value ?? string.Empty;
        }
    }
}
