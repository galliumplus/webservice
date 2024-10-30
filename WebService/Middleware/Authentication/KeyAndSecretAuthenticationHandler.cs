using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using GalliumPlus.Core.Applications;
using GalliumPlus.Core.Data;
using GalliumPlus.Core.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace GalliumPlus.WebService.Middleware.Authentication
{
    public class KeyAndSecretAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private IClientDao clients;

        public KeyAndSecretAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IClientDao clients)
        : base(options, logger, encoder)
        {
            this.clients = clients;
        }

        private bool TryParseHeader(out string secret)
        {
            secret = "";
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(this.Request.Headers.Authorization.ToString());

                if (authHeader.Scheme != "Secret") return false;

                secret = authHeader.Parameter ?? "";
                return true;
            }
            catch
            {
                return false;
            }
        }

#pragma warning disable CS1998 // Cette méthode async n'a pas d'opérateur 'await' et elle s'exécutera de façon synchrone
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string secret;
            if (!this.Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing header");
            }
            else if (!this.TryParseHeader(out secret))
            {
                return AuthenticateResult.Fail("Invalid header format");
            }

            if (!ApiKey.Find(out string? apiKey, this.Request.Headers))
            {
                return AuthenticateResult.Fail("Missing API key");
            }

            Client bot;
            try
            {
                bot = this.clients.FindByApiKeyWithAppAccess(apiKey);
            }
            catch (ItemNotFoundException)
            {
                return AuthenticateResult.Fail("Invalid API key");
            }

            if (!bot.AppAccess!.SecretsMatch(secret))
            {
                return AuthenticateResult.Fail("Secrets doesn't match");
            }

            this.Context.Items.Add("Client", bot);

            return AuthenticateResult.Success(new EmptyTicket(this.Scheme));
        }
    }
#pragma warning restore CS1998 // Cette méthode async n'a pas d'opérateur 'await' et elle s'exécutera de façon synchrone

    public static class KeyAndSecretAuthenticationExtensions
    {
        public static AuthenticationBuilder AddKeyAndSecret(this AuthenticationBuilder builder)
        {
            return builder.AddScheme<AuthenticationSchemeOptions, KeyAndSecretAuthenticationHandler>("KeyAndSecret", null);
        }
    }
}
