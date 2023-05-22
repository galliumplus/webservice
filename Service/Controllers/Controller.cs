using GalliumPlus.WebApi.Core;
using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Users;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GalliumPlus.WebApi.Controllers
{
    /// <summary>
    /// Contrôleur de base.
    /// </summary>
    public class Controller : ControllerBase
    {
        /// <summary>
        /// L'utilisateur qui a émis la requête actuelle.
        /// </summary>
        public new User? User
        {
            get
            {
                User? result = null;

                if (this.HttpContext.Items.TryGetValue("User", out object? value))
                {
                    if (value is User user)
                    {
                        result = user;
                    }
                    else if (value is not null)
                    {
                        throw new InvalidOperationException(
                            "The \"User\" item must be of type GalliumPlus.WebApi.Core.Users.User, "
                            + $"not {value.GetType().FullName}"
                        );
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// La session de l'utilisateur qui a émis la requête actuelle.
        /// </summary>
        public Session? Session
        {
            get
            {
                Session? result = null;

                if (this.HttpContext.Items.TryGetValue("Session", out object? value))
                {
                    if (value is Session session)
                    {
                        result = session;
                    }
                    else if (value is not null)
                    {
                        throw new InvalidOperationException(
                            "The \"Session\" item must be of type GalliumPlus.WebApi.Core.Users.Session, "
                            + $"not {value.GetType().FullName}"
                        );
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Crée une réponse avec un corps JSON.
        /// </summary>
        /// <param name="value">L'objet à sérialiser.</param>
        /// <param name="statusCode">Le statut de la réponse.</param>
        [NonAction]
        public IActionResult Json(object? value, int statusCode = StatusCodes.Status200OK)
        {
            return new JsonResult(value) { StatusCode = statusCode };
        }

        /// <summary>
        /// Crée une réponse avec un statut de 201.
        /// </summary>
        /// <param name="route">La route de la ressource crée.</param>
        /// <param name="routeValues">Les paramètres de la route.</param>
        /// <param name="value">Le ressource crée.</param>
        [NonAction]
        public IActionResult Created(string route, object routeValues, object? value = null)
        {
            return new CreatedAtRouteResult(route, routeValues, value);
        }

        /// <summary>
        /// Crée une réponse avec un statut de 201.
        /// </summary>
        /// <param name="route">La route de la ressource crée.</param>
        /// <param name="id">Le paramètre id de la route.</param>
        /// <param name="value">Le ressource crée.</param>
        [NonAction]
        public IActionResult Created(string route, int id, object? value = null)
            => Created(route, new { id }, value);

        /// <summary>
        /// Crée une réponse avec un statut de 201.
        /// </summary>
        /// <param name="route">La route de la ressource crée.</param>
        /// <param name="id">Le paramètre id de la route.</param>
        /// <param name="value">Le ressource crée.</param>
        [NonAction]
        public IActionResult Created(string route, string id, object? value = null)
            => Created(route, new { id }, value);

        [NonAction]
        public void RequirePermissions(Permissions required)
        {
            if (!this.User!.Role.Permissions.Includes(required))
            {
                throw new PermissionDeniedException(required);
            }
        }
    }
}
