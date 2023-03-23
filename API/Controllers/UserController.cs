using GalliumPlusAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlusAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : Controller
    {
        [HttpGet("me")]
        public IActionResult GetSignedIn()
        {
            return Json(
                new User
                {
                    Id = "mf18xxxx",
                    Name = "Fatéo Mavard",
                    Deposit = 5.30,
                    Role = new Role { Id = 0, Name = "CA", Permissions = (int)(Permission.SEE_ALL_USERS | Permission.SELL) },
                    Year = "2A",
                    RequireValidationForPayments = false,
                }
            );
        }
    }
}
