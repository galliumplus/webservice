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
    public class BearerAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private ISessionDao sessions;
        private readonly GalliumOptions galliumOptions;

        public BearerAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISessionDao sessions,
            GalliumOptions galliumOptions)
        : base(options, logger, encoder)
        {
            this.sessions = sessions;
            this.galliumOptions = galliumOptions;
        }

        protected bool TryParseHeader(out string token)
        {
            token = "";

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(this.Request.Headers.Authorization.ToString());

                if (authHeader.Scheme != "Bearer") return false;

                token = authHeader.Parameter!;

                return true;
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

            string token;
            if (!this.TryParseHeader(out token))
            {
                return AuthenticateResult.Fail("Invalid header format");
            }

            Session session;
            try
            {
                session = this.sessions.Read(token);
            }
            catch (ItemNotFoundException)
            {
                return AuthenticateResult.Fail("Session not found");
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Unexpected error while reading session");
                return AuthenticateResult.Fail("Error 500");
            }

            if (session.Token == null)
            {
                return AuthenticateResult.NoResult();
            }
            
            if (!session.Refresh(this.galliumOptions.Session))
            {
                this.sessions.Delete(session);
                return AuthenticateResult.Fail("Session expired");
            }
            this.sessions.UpdateLastUse(session);

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
