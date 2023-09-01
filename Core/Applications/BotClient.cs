using GalliumPlus.WebApi.Core.Application;
using GalliumPlus.WebApi.Core.Users;

namespace GalliumPlus.WebApi.Core.Applications
{
    /// <summary>
    /// Représente un client automatisé.
    /// </summary>
    public class BotClient : Client
    {
        private OneTimeSecret secret;

        /// <summary>
        /// Le code secret servant à authentifier le bot.
        /// </summary>
        public OneTimeSecret Secret => this.secret;

        /// <summary>
        /// Crée un bot existant.
        /// </summary>
        /// <param name="id">L'identifiant du bot.</param>
        /// <param name="apiKey">La clé d'API.</param>
        /// <param name="secret">Le code secret.</param>
        /// <param name="name">Le nom du bot.</param>
        /// <param name="isEnabled">Si l'application est active ou non.</param>
        /// <param name="permissions">Les permissions accordées au bot.</param>
        public BotClient(int id, string apiKey, OneTimeSecret secret, string name, bool isEnabled, Permissions permissions)
        : base(id, apiKey, name, isEnabled, permissions, Permissions.NONE, false)
        {
            this.secret = secret;
        }

        /// <summary>
        /// Crée un nouveau bot.
        /// </summary>
        /// <param name="name">Le nom du bot.</param>
        /// <param name="permissions">Les permissions accordées au bot.</param>
        public BotClient(string name, bool isEnabled, Permissions permissions)
        : base(name, isEnabled, granted: permissions, allowUsers: false)
        {
            this.secret = new OneTimeSecret();
        }
    }
}
