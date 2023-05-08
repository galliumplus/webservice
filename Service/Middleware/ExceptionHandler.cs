using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using GalliumPlus.WebApi.Core;
using System.Text.Json.Serialization;

namespace GalliumPlus.WebApi.Middleware
{
    /// <summary>
    /// Gestion des erreurs propres au code métier.
    /// </summary>
    public class ExceptionHandler : IActionFilter, IOrderedFilter
    {
        /// <summary>
        /// Représente une erreur.
        /// </summary>
        private class Error
        {
            /// <summary>
            /// Un code identifiant le type d'erreur, composé de lettres majuscules et de soulignés.
            /// </summary>
            public string Code { get; set; } = "UNKNOWN";

            /// <summary>
            /// Une phrase expliquant l'erreur.
            /// </summary>
            public string Message { get; set; } = "Erreur inconnue.";

            /// <summary>
            /// Un objet additionel contenant des informations sur l'erreur
            /// (utile pour le déboguage).
            /// </summary>
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public object? ErrorData { get; set; } = null;
        }

        /// <summary>
        /// Construit une réponse d'erreur.
        /// </summary>
        /// <param name="errorCode">Le code de l'erreur (voir <see cref="Error.Code"/>)</param>
        /// <param name="errorMessage">Le message d'erreur (voir <see cref="Error.Message"/>)</param>
        /// <param name="statusCode">Le statut HTTP de la réponse.</param>
        /// <param name="data">Données additionnelles pour le déboguage</param>
        /// <returns></returns>
        private static IActionResult BuildErrorResult(
            string errorCode,
            string errorMessage,
            int statusCode,
            object? errorData = null)
        {
            Error error = new Error { Code = errorCode, Message = errorMessage, ErrorData = errorData };
            return new JsonResult(error) { StatusCode = statusCode };
        }

        public static void ConfigureInvalidModelStateResponseFactory(ApiBehaviorOptions options)
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                List<string> modelStateErrors = new();
                foreach (var modelStateEntry in context.ModelState)
                {
                    foreach (var error in modelStateEntry.Value.Errors)
                    {
                        modelStateErrors.Add(error.ErrorMessage);
                    }
                }

                return BuildErrorResult(
                    "INVALID_ITEM",
                    "Le format de cette ressource est invalide.",
                    400,
                    new { ModelStateErrors = modelStateErrors }
                );
            };
        }

        // priorité haute, on veut qu'il s'applique juste après le contrôleur
        public int Order => 1_000_000; 

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is ItemNotFoundException)
            {
                context.Result = BuildErrorResult(
                    "ITEM_NOT_FOUND",
                    "La ressource demandée n'existe pas.",
                    404
                );
                context.ExceptionHandled = true;
            }
            else if (context.Exception is InvalidItemException invalidItem)
            {
                context.Result = BuildErrorResult(
                    "INVALID_ITEM",
                    invalidItem.Message,
                    400
                );
                context.ExceptionHandled = true;
            }
            else if (context.Exception is DuplicateItemException duplicateItem)
            {
                context.Result = BuildErrorResult(
                    "DUPLICATE_ITEM",
                    "Cette ressource existe déjà.",
                    400
                );
                context.ExceptionHandled = true;
            }
        }
    }
}
