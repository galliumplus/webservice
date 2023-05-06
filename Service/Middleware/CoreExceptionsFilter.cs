using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using GalliumPlus.WebApi.Core;

namespace GalliumPlus.WebApi.Middleware
{
    /// <summary>
    /// Gestion des erreurs propres au code métier.
    /// </summary>
    public class CoreExceptionsFilter : IActionFilter, IOrderedFilter
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
        }

        // priorité haute, on veut qu'il s'applique juste après le contrôleur
        public int Order => 1_000_000; 

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is ItemNotFoundException)
            {
                Error error = new Error
                {
                    Code = "ITEM_NOT_FOUND",
                    Message = "La ressource demandée n'existe pas.",
                };

                context.Result = new ObjectResult(error) { StatusCode = 404 };
                context.ExceptionHandled = true;
            }
            else if (context.Exception is InvalidItemException invalidItem)
            {
                Error error = new Error
                {
                    Code = "INVALID_ITEM",
                    Message = invalidItem.Message
                };

                context.Result = new ObjectResult(error) { StatusCode = 400 };
                context.ExceptionHandled = true;
            }
        }
    }
}
