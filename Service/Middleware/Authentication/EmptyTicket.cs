using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace GalliumPlus.WebApi.Middleware.Authentication
{
    /// <summary>
    /// Un ticket d'authentification sans informations particulières.
    /// Les informations sur l'utilisateur sont passées directement au
    /// <see cref="HttpContext"/>.
    /// </summary>
    public class EmptyTicket : AuthenticationTicket
    {
        /// <summary>
        /// Crée un ticket d'authentification.
        /// </summary>
        /// <param name="authenticationScheme">Le schéma utilisé pour authentifier l'utilisateur.</param>
        public EmptyTicket(AuthenticationScheme authenticationScheme)
        : base(new ClaimsPrincipal(new ClaimsIdentity(authenticationScheme.Name)), authenticationScheme.Name)
        {
        }
    }
}