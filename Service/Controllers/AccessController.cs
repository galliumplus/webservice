using GalliumPlus.WebApi.Core.Data;
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
            return Ok();
        }
    }
}
