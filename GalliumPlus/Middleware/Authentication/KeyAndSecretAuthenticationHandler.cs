﻿using GalliumPlus.WebApi.Core.Applications;
using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;

namespace GalliumPlus.WebApi.Middleware.Authentication
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
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers.Authorization.ToString());

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
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing header");
            }
            else if (!TryParseHeader(out secret))
            {
                return AuthenticateResult.Fail("Invalid header format");
            }

            if (!ApiKey.Find(out string? apiKey, Request.Headers))
            {
                return AuthenticateResult.Fail("Missing API key");
            }

            BotClient bot;
            try
            {
                bot = clients.FindBotByApiKey(apiKey);
            }
            catch (ItemNotFoundException)
            {
                return AuthenticateResult.Fail("Invalid API key");
            }

            if (!bot.Secret.Match(secret))
            {
                return AuthenticateResult.Fail("Secret don't match");
            }

            Context.Items.Add("Client", bot);

            return AuthenticateResult.Success(new EmptyTicket(Scheme));
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
