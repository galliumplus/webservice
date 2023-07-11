using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreTest.Applications
{
    public class BotClientTest
    {
        [Fact]
        public void ConstructorExisting()
        {
            BotClient bot = new(
                apiKey: "bot-api-key",
                secret: "bot-secret-token",
                name: "Bot",
                isEnabled: true,
                permissions: Permissions.RESET_MEMBERSHIPS
            );

            Assert.Equal("bot-api-key", bot.ApiKey);
            Assert.Equal("bot-secret-token", bot.Secret);
            Assert.Equal("Bot", bot.Name);
            Assert.True(bot.IsEnabled);
            Assert.Equal(Permissions.RESET_MEMBERSHIPS, bot.Granted);
            Assert.Equal(Permissions.NONE, bot.Revoked);
        }

        [Fact]
        public void ConstructorNew()
        {
            BotClient bot = new(
                name: "Bot",
                permissions: Permissions.RESET_MEMBERSHIPS
            );

            Assert.Matches(ClientTest.RE_API_KEY, bot.ApiKey);
            Assert.Matches(ClientTest.RE_SECRET, bot.Secret);
            Assert.Equal("Bot", bot.Name);
            Assert.True(bot.IsEnabled);
            Assert.Equal(Permissions.RESET_MEMBERSHIPS, bot.Granted);
            Assert.Equal(Permissions.NONE, bot.Revoked);
        }

        [Fact]
        public void RegenerateSecret()
        {
            BotClient bot = new(
                name: "Bot",
                permissions: Permissions.RESET_MEMBERSHIPS
            );

            string previousSecret = bot.Secret;

            bot.RegenerateSecret();

            Assert.NotEqual(previousSecret, bot.Secret);
            Assert.Matches(ClientTest.RE_SECRET, bot.Secret);
        }
    }
}
