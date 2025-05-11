using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Security;
using GalliumPlus.Core.Users;
using GalliumPlus.WebService.Middleware.ErrorHandling;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GalliumPlus.WebService.Middleware.Authorization
{
    /// <summary>
    /// Autorise ou non les requêtes en se basant sur le système de permissions de Gallium.
    /// </summary>
    public class PermissionsFilter : IAuthorizationFilter
    {
        /// <summary>
        /// Récupère les permissions requises par une action.
        /// </summary>
        /// <param name="action">L'action en question.</param>
        /// <returns>Les permissions demandées.</returns>
        private static Permission RequiresPermissions(ActionDescriptor action)
        {
            foreach (object metadata in action.EndpointMetadata)
            {
                if (metadata is RequiresPermissionsAttribute permissions)
                {
                    return permissions.Required;
                }
            }
            return Permission.None;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            Permission required = RequiresPermissions(context.ActionDescriptor);

            // OK, aucune permission demandée
            if (required == Permission.None) return;

            Session session = (Session)context.HttpContext.Items["Session"]!;

            // OK, l'utilisateur a toutes les permissions nécessaires
            if (Permissions.Current.IsSupersetOf(session.Permissions, required)) return;

            string messageAction = context.HttpContext.Request.Method switch
            {
                "GET" or "HEAD" => "d'accéder à cette ressource",
                _ => "d'effectuer cette action",
            };

            context.Result = new ErrorResult(
                ErrorCode.PermissionDenied,
                $"Vous n'avez pas la permission {messageAction}.",
                StatusCodes.Status403Forbidden,
                new { RequiredPermissions = required }
            );
        }
    }
}
