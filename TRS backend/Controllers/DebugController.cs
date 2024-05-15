using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            return HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
        }
    }
}
