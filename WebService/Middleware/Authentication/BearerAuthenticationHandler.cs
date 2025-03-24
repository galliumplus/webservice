using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using GalliumPlus.Core.Data;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

#pragma warning disable CS1998 // Cette méthode async n'a pas d'opérateur 'await' et elle s'exécutera de façon synchrone

namespace GalliumPlus.WebService.Middleware.Authentication
{
    public class BearerAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISessionDao sessions,
        GalliumOptions galliumOptions)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        private bool TryParseHeader(out string token)
        {
            token = "";

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(this.Request.Headers.Authorization.ToString());
                if (authHeader.Scheme != "Bearer")
                {
                    return false;
                }
                else
                {
                    token = authHeader.Parameter ?? "";
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!this.Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing Authorization header");
            }

            if (!this.TryParseHeader(out string token))
            {
                return AuthenticateResult.Fail("Invalid header format");
            }

            Session session;
            try
            {
                session = sessions.Read(token);
            }
            catch (ItemNotFoundException)
            {
                return AuthenticateResult.Fail("Session not found");
            }
            
            if (!session.Refresh(galliumOptions.Session))
            {
                sessions.Delete(session);
                return AuthenticateResult.Fail("Session expired");
            }
            sessions.UpdateLastUse(session);

            this.Context.Items.Add("Session", session);
            this.Context.Items.Add("User", session.User);
            this.Context.Items.Add("Client", session.Client);

            return AuthenticateResult.Success(new EmptyTicket(this.Scheme));
        }
    }

    public static class BearerAuthenticationExtensions
    {
        public static AuthenticationBuilder AddBearer(this AuthenticationBuilder builder)
        {
            return builder.AddScheme<AuthenticationSchemeOptions, BearerAuthenticationHandler>("Bearer", null);
        }
    }
}
