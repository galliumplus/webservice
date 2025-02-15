using GalliumPlus.Core.Applications;
using GalliumPlus.Core.Data;
using GalliumPlus.Core.Logs;
using GalliumPlus.Core.Users;
using GalliumPlus.WebService.Dto.Access;
using GalliumPlus.WebService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GalliumPlus.WebService.Controllers;

[Route("v1")]
[ApiController]
public class AccessController(AccessService service, ISessionDao sessionDao, IHistoryDao historyDao) : GalliumController
{
    [HttpPost("login")]
    [Authorize(AuthenticationSchemes = "Basic")]
    public IActionResult LogIn()
    {
        Client app = this.Client!;
        User user = this.User!;

        LoggedIn? loggedIn = service.LogIn(app, user);

        if (loggedIn == null)
        {
            return this.StatusCode(StatusCodes.Status503ServiceUnavailable);
        }
        else
        {
            HistoryAction action = new(
                HistoryActionKind.LogIn,
                $"Connexion à {app.Name}",
                user.Id
            );
            historyDao.AddEntry(action);

            return this.Json(loggedIn);
        }
    }

    [HttpPost("connect")]
    [Authorize(AuthenticationSchemes = "KeyAndSecret")]
    public IActionResult Connect()
    {
        Client bot = this.Client!;

        LoggedIn? loggedIn = service.ConnectApplication(bot);

        if (loggedIn == null)
        {
            return this.StatusCode(StatusCodes.Status503ServiceUnavailable);
        }
        else
        {
            HistoryAction action = new(
                HistoryActionKind.LogIn,
                $"Connexion de {bot.Name}"
            );
            historyDao.AddEntry(action);

            return this.Json(loggedIn);
        }
    }

    [HttpPost("same-sign-on")]
    [Authorize(AuthenticationSchemes = "Basic")]
    public IActionResult SameSignOn(GalliumOptions options)
    {
        this.HttpContext.Items.TryGetValue("SsoClientKey", out object? appKey);
        LoggedInThroughSso? session = service.SameSignOn(this.User!, (string)appKey!, options.Host);
            
        if (session == null)
        {
            return this.StatusCode(StatusCodes.Status503ServiceUnavailable);
        }
        else
        {
            HistoryAction action = new(
                HistoryActionKind.LogIn,
                $"Connexion à {session.App.Name} ({session.RedirectUrl}) via le portail de {this.Client!.Name}",
                this.User?.Id
            );
            historyDao.AddEntry(action);

            return this.Json(session);
        }
    }

    [HttpPost("logout")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public IActionResult LogOut()
    {
        sessionDao.Delete(this.Session!);
        return this.Ok();
    }
}