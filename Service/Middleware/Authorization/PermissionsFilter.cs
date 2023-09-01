using GalliumPlus.WebApi.Core.Users;
using GalliumPlus.WebApi.Middleware.ErrorHandling;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GalliumPlus.WebApi.Middleware.Authorization
{
    /// <summary>
    /// Autorise ou non les requêtes en se basant sur le système de permissions de Gallium.
    /// </summary>
    public class PermissionsFilter : IAuthorizationFilter
    {
        /// <summary>
        /// Récupère les permissions requise par une action.
        /// </summary>
        /// <param name="action">L'action en question.</param>
        /// <returns>Les permissions demandées.</returns>
        private static Permissions RequiresPermissions(ActionDescriptor action)
        {
            foreach (object metadata in action.EndpointMetadata)
            {
                if (metadata is RequiresPermissionsAttribute permissions)
                {
                    return permissions.Required;
                }
            }
            return Permissions.NONE;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            Permissions required = RequiresPermissions(context.ActionDescriptor);

            // OK, aucune permission demandée
            if (required == Permissions.NONE) return;

            Session session = (Session)context.HttpContext.Items["Session"]!;

            // OK, l'utilisateur a toutes les permissions nécéssaires
            if (session.Permissions.Includes(required)) return;

            string messageAction = context.HttpContext.Request.Method switch
            {
                "GET" or "HEAD" => "d'accéder à cette ressource",
                _ => "d'effectuer cette action",
            };

            context.Result = new ErrorResult(
                "PERMISSION_DENIED",
                $"Vous n'avez pas la permission {messageAction}.",
                StatusCodes.Status403Forbidden,
                new { RequiredPermissions = required }
            );
        }
    }
}
