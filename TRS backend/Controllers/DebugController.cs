using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using TRS_backend.DBModel;

namespace TRS_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DebugController : Controller
    {
        [Authorize]
        [HttpGet("Role")]
        public ActionResult<string> Role()
        {
            Claim claim = null!;
            try {
                claim = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Role);
            }
            catch { }
            
            if (claim == null || claim.Value.IsNullOrEmpty()) {
                return Ok("Not authorized");
            }
            return claim.Value;
        }
    }
}
