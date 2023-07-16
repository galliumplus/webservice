using GalliumPlus.WebApi.Core;
using GalliumPlus.WebApi.Core.Applications;
using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Users;
using GalliumPlus.WebApi.Dto;
using GalliumPlus.WebApi.Middleware.ErrorHandling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class AccessController : Controller
    {
        private ISessionDao sessionDao;
        private LoggedIn.Mapper mapper = new();

        public AccessController(ISessionDao sessionDao)
        {
            this.sessionDao = sessionDao;
        }

        [HttpPost("login")]
        [Authorize(AuthenticationSchemes = "Basic")]
        public IActionResult LogIn()
        {
            Client app = this.Client!;
            User user = this.User!;

            if (!app.AllowUserLogin)
            {
                return new ErrorResult(
                   "LOGIN_DENIED",
                   $"Vous ne pouvez pas vous connecter via {app.Name}.",
                   StatusCodes.Status403Forbidden,
                   new { AppEnabled = app.IsEnabled, UserLoginAllowed = app.AllowUsers }
               );
            }

            Permissions sessionPerms = app.Filter(user.Role.Permissions);

            for (int tries = 10; tries > 0; tries--)
            {
                try
                {
                    Session session = this.sessionDao.Create(Session.LogIn(user, sessionPerms));
                    return Json(this.mapper.FromModel(session));
                }
                catch (DuplicateItemException) { }
            }
            return StatusCode(StatusCodes.Status503ServiceUnavailable);
        }

        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult LogOut()
        {
            this.sessionDao.Delete(this.Session!.Token);
            return Ok();
        }
    }
}
