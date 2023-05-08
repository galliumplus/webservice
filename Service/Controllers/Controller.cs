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
        private IMasterDao dao;

        /// <summary>
        /// Le DAO général pour toutes les ressources.
        /// </summary>
        protected IMasterDao Dao => dao;

        public Controller(IMasterDao dao)
        {
            this.dao = dao;
        }

        /// <summary>
        /// Crée une réponse avec un corps JSON.
        /// </summary>
        /// <param name="value">L'objet à sérialiser.</param>
        /// <param name="statusCode">Le statut de la réponse.</param>
        public IActionResult Json(object? value, int statusCode = StatusCodes.Status200OK)
        {
            return new JsonResult(value) { StatusCode = statusCode };
        }

        /// <summary>
        /// Crée une réponse avec un statut de 201, sans informations sur la ressource créée.
        /// </summary>
        public IActionResult Created() => new StatusCodeResult(StatusCodes.Status201Created);
    }
}
