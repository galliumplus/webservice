using GalliumPlus.WebApi.Core.Data;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class AccessController : Controller
    {
        public AccessController(IMasterDao dao) : base(dao) { }

        [HttpPost("signin")]
        public IActionResult SignIn()
        {
            return Ok();
        }
    }
}
