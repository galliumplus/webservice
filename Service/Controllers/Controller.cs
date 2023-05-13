using GalliumPlus.WebApi.Core.Data;
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
        /// Crée une réponse avec un corps JSON.
        /// </summary>
        /// <param name="value">L'objet à sérialiser.</param>
        /// <param name="statusCode">Le statut de la réponse.</param>
        protected IActionResult Json(object? value, int statusCode = StatusCodes.Status200OK)
        {
            return new JsonResult(value) { StatusCode = statusCode };
        }

        /// <summary>
        /// Crée une réponse avec un statut de 201.
        /// </summary>
        /// <param name="route">La route de la ressource crée.</param>
        /// <param name="routeValues">Les paramètres de la route.</param>
        /// <param name="value">Le ressource crée.</param>
        protected IActionResult Created(string route, object routeValues, object? value = null)
        {
            return new CreatedAtRouteResult(route, routeValues, value);
        }

        /// <summary>
        /// Crée une réponse avec un statut de 201.
        /// </summary>
        /// <param name="route">La route de la ressource crée.</param>
        /// <param name="id">Le paramètre id de la route.</param>
        /// <param name="value">Le ressource crée.</param>
        protected IActionResult Created(string route, int id, object? value = null)
            => Created(route, new { id }, value);

        /// <summary>
        /// Crée une réponse avec un statut de 201.
        /// </summary>
        /// <param name="route">La route de la ressource crée.</param>
        /// <param name="id">Le paramètre id de la route.</param>
        /// <param name="value">Le ressource crée.</param>
        protected IActionResult Created(string route, string id, object? value = null)
            => Created(route, new { id }, value);
    }
}
