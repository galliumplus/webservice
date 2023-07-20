using GalliumPlus.WebApi.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GalliumPlus.WebApi.Middleware.ErrorHandling
{
    /// <summary>
    /// Gestion des erreurs propres au code métier.
    /// </summary>
    public class ExceptionsFilter : IExceptionFilter, IOrderedFilter
    {
        // priorité haute, on veut qu'il s'applique juste après le contrôleur
        public int Order => 1_000_000;

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ItemNotFoundException)
            {
                context.Result = new ErrorResult(
                    "ITEM_NOT_FOUND",
                    "La ressource demandée n'existe pas.",
                    StatusCodes.Status404NotFound
                );
                context.ExceptionHandled = true;
            }
            else if (context.Exception is InvalidItemException invalidItem)
            {
                context.Result = new ErrorResult(
                    "INVALID_ITEM",
                    invalidItem.Message,
                    StatusCodes.Status400BadRequest
                );
                context.ExceptionHandled = true;
            }
            else if (context.Exception is DuplicateItemException)
            {
                context.Result = new ErrorResult(
                    "DUPLICATE_ITEM",
                    "Cette ressource existe déjà.",
                    StatusCodes.Status400BadRequest
                );
                context.ExceptionHandled = true;
            }
            else if (context.Exception is PermissionDeniedException permissionDenied)
            {
                string messageAction = context.HttpContext.Request.Method switch
                {
                    "GET" or "HEAD" => "d'accéder à cette ressource",
                    _ => "d'effectuer cette action",
                };

                context.Result = new ErrorResult(
                    "PERMISSION_DENIED",
                    $"Vous n'avez pas la permission {messageAction}.",
                    StatusCodes.Status403Forbidden,
                    new { RequiredPermissions = permissionDenied.Required }
                );
                context.ExceptionHandled = true;
            }
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

                return new ErrorResult(
                    "INVALID_ITEM",
                    "Le format de cette ressource est invalide.",
                    400,
                    new { ModelStateErrors = modelStateErrors }
                );
            };
        }
    }
}
