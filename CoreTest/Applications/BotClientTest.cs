using GalliumPlus.WebApi.Core;

namespace CoreTest.Applications
{
    public class BotClientTest
    {
        [Fact]
        public void ConstructorExisting()
        {
            OneTimeSecret secret = new();
            string expectedSecret = secret.Regenerate();

            BotClient bot = new(
                id: 123,
                apiKey: "bot-api-key",
                secret,
                name: "Bot",
                isEnabled: true,
                permissions: Permissions.RESET_MEMBERSHIPS
            );

            Assert.Equal(123, bot.Id);
            Assert.Equal("bot-api-key", bot.ApiKey);
            Assert.True(bot.Secret.Match(expectedSecret));
            Assert.Equal("Bot", bot.Name);
            Assert.True(bot.IsEnabled);
            Assert.Equal(Permissions.RESET_MEMBERSHIPS, bot.Granted);
            Assert.Equal(Permissions.NONE, bot.Revoked);
            Assert.False(bot.AllowUserLogin);
        }

        [Fact]
        public void ConstructorNew()
        {
            BotClient bot = new(
                name: "Bot",
                permissions: Permissions.RESET_MEMBERSHIPS,
                isEnabled: true
            );

            Assert.Matches(ClientTest.RE_API_KEY, bot.ApiKey);
            Assert.Equal("Bot", bot.Name);
            Assert.True(bot.IsEnabled);
            Assert.Equal(Permissions.RESET_MEMBERSHIPS, bot.Granted);
            Assert.Equal(Permissions.NONE, bot.Revoked);
            Assert.False(bot.AllowUserLogin);
        }

        [Fact]
        public void RegenerateSecret()
        {
            BotClient bot = new(
                name: "Bot",
                permissions: Permissions.RESET_MEMBERSHIPS,
                isEnabled: true
            );

            string previousSecret = bot.Secret.Regenerate();

            string newSecret = bot.Secret.Regenerate();

            Assert.NotEqual(previousSecret, newSecret);
        }
    }
}
