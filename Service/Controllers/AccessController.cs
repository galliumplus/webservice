using GalliumPlus.WebApi.Core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class AccessController : Controller
    {
        [HttpPost("signin")]
        public IActionResult SignIn()
        {
            return Json(HttpContext.Items["User"]);
        }
    }
}
