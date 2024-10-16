using GalliumPlus.Core.Applications;
using GalliumPlus.Core.Data;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.History;
using GalliumPlus.Core.Users;
using GalliumPlus.WebService.Dto;
using GalliumPlus.WebService.Middleware.ErrorHandling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Controller = GalliumPlus.WebService.Controllers.Controller;

namespace GalliumPlus.WebService.Controllers
{
    [Route("v1")]
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
                   ErrorCode.PermissionDenied,
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
                        HistoryActionKind.LogIn,
                        $"Connexion à {app.Name}",
                        user.Id
                    );
                    this.historyDao.AddEntry(action);

                    return this.Json(this.mapper.FromModel(session));
                }
                catch (DuplicateItemException) { }
            }
            return this.StatusCode(StatusCodes.Status503ServiceUnavailable);
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
                        HistoryActionKind.LogIn,
                        $"Connexion de {bot.Name}"
                    );
                    this.historyDao.AddEntry(action);

                    return this.Json(this.mapper.FromModel(session));
                }
                catch (DuplicateItemException) { }
            }
            return this.StatusCode(StatusCodes.Status503ServiceUnavailable);
        }

        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult LogOut()
        {
            this.sessionDao.Delete(this.Session!);
            return this.Ok();
        }
    }
}
