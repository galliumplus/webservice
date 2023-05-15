using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using GalliumPlus.WebApi.Core;
using System.Text.Json.Serialization;

namespace GalliumPlus.WebApi.Middleware.Authorization
{
    /// <summary>
    /// Gestion des erreurs propres au code métier.
    /// </summary>
    public class PermissionsFilter : IActionFilter, IOrderedFilter
    {
        // priorité basse, on veut qu'il s'applique le plus tôt possible
        public int Order => -1_000_000;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (object metadata in context.ActionDescriptor.EndpointMetadata)
            {
                if (metadata is RequirePermissionsAttribute)
                { 
                    throw new NotImplementedException();
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
