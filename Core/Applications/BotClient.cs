using GalliumPlus.WebApi.Core.Random;
using GalliumPlus.WebApi.Core.Users;

namespace GalliumPlus.WebApi.Core.Applications
{
    /// <summary>
    /// Représente un client automatisé.
    /// </summary>
    public class BotClient : Client
    {
        private string secret;

        /// <summary>
        /// Le code secret servant à authentifier le bot.
        /// </summary>
        public string Secret => this.secret;

        /// <summary>
        /// Crée un bot existant.
        /// </summary>
        /// <param name="apiKey">La clé d'API.</param>
        /// <param name="secret">Le code secret.</param>
        /// <param name="name">Le nom du bot.</param>
        /// <param name="isEnabled">Si l'application est active ou non.</param>
        /// <param name="permissions">Les permissions accordées au bot.</param>
        public BotClient(string apiKey, string secret, string name, bool isEnabled, Permissions permissions)
        : base(apiKey, name, isEnabled, permissions, Permissions.NONE, false)
        {
            this.secret = secret;
        }

        /// <summary>
        /// Crée un nouveau bot.
        /// </summary>
        /// <param name="name">Le nom du bot.</param>
        /// <param name="permissions">Les permissions accordées au bot.</param>
        public BotClient(string name, Permissions permissions)
        : base(name, granted: permissions, allowUsers: false)
        {
            this.secret = "";
            this.RegenerateSecret();
        }

        public void RegenerateSecret()
        {
            var rtg = new RandomTextGenerator(new CryptoRandomProvider());
            this.secret = rtg.SecretKey();
        }
    }
}
