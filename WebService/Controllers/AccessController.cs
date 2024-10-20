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
    public class AccessController(AccessService service, ISessionDao sessionDao, IHistoryDao historyDao) : Controller
    {
        private LoggedIn.Mapper mapper = new();

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
                    sessionDao.Create(session);

                    HistoryAction action = new(
                        HistoryActionKind.LogIn,
                        $"Connexion à {app.Name}",
                        user.Id
                    );
                    historyDao.AddEntry(action);

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
                    sessionDao.Create(session);

                    HistoryAction action = new(
                        HistoryActionKind.LogIn,
                        $"Connexion de {bot.Name}"
                    );
                    historyDao.AddEntry(action);

                    return this.Json(this.mapper.FromModel(session));
                }
                catch (DuplicateItemException) { }
            }
            return this.StatusCode(StatusCodes.Status503ServiceUnavailable);
        }

        [HttpPost("same-sign-on")]
        [Authorize(AuthenticationSchemes = "Basic")]
        public IActionResult SameSignOn(GalliumOptions options)
        {
            this.HttpContext.Items.TryGetValue("SsoClientKey", out object? appKey);
            var session = service.SameSignOn(this.Client!, this.User!, (string)appKey!, options.Host);
            
            HistoryAction action = new(
                HistoryActionKind.LogIn,
                $"Connexion à une appli externe ({session.RedirectUrl}) via le portail de {this.Client!.Name}",
                this.User!.Id
            );
            historyDao.AddEntry(action);
            
            return Json(session);
        }

        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult LogOut()
        {
            sessionDao.Delete(this.Session!);
            return this.Ok();
        }
    }
}
