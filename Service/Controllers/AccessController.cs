using GalliumPlus.WebApi.Core.Applications;
using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.History;
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
        private IHistoryDao historyDao;
        private LoggedIn.Mapper mapper;

        public AccessController(ISessionDao sessionDao, IHistoryDao historyDao)
        {
            this.sessionDao = sessionDao;
            this.historyDao = historyDao;
            this.mapper = new();
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
                   ErrorCode.PERMISSION_DENIED,
                   $"Vous ne pouvez pas vous connecter directement à {app.Name}.",
                   StatusCodes.Status403Forbidden,
                   new { AppEnabled = app.IsEnabled, UserLoginAllowed = app.AllowUserLogin }
               );
            }

            for (int tries = 10; tries > 0; tries--)
            {
                try
                {
                    Session session = Session.LogIn(app, user);
                    this.sessionDao.Create(session);

                    HistoryAction action = new(
                        HistoryActionKind.LOG_IN,
                        $"Connexion à {app.Name}",
                        user.Id
                    );
                    this.historyDao.AddEntry(action);

                    return Json(this.mapper.FromModel(session));
                }
                catch (DuplicateItemException) { }
            }
            return StatusCode(StatusCodes.Status503ServiceUnavailable);
        }

        [HttpPost("connect")]
        [Authorize(AuthenticationSchemes = "KeyAndSecret")]
        public IActionResult Connect()
        {
            Client bot = this.Client!;

            for (int tries = 10; tries > 0; tries--)
            {
                try
                {
                    Session session = Session.LogIn(bot);
                    this.sessionDao.Create(session);

                    HistoryAction action = new(
                        HistoryActionKind.LOG_IN,
                        $"Connexion de {bot.Name}"
                    );
                    this.historyDao.AddEntry(action);

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
            this.sessionDao.Delete(this.Session!);
            return Ok();
        }
    }
}
