using GalliumPlus.WebApi.Core.Random;
using GalliumPlus.WebApi.Core.Users;

namespace GalliumPlus.WebApi.Core.Applications
{
    /// <summary>
    /// Réprésente une application depuis laquelle l'utilisateur utilise l'API.
    /// </summary>
    public class Client
    {
        private string apiKey;
        private string name;
        private Permissions granted;
        private Permissions revoked;
        private bool isEnabled;
        private bool allowUsers;

        /// <summary>
        /// La clé d'API de l'application
        /// </summary>
        public string ApiKey { get => this.apiKey; set => this.apiKey = value; }

        /// <summary>
        /// Le nom servant à identifier rapidement l'application.
        /// </summary>
        public string Name => this.name;

        /// <summary>
        /// Les permissions données à tous les utilisteurs de l'application.
        /// </summary>
        public Permissions Granted => this.granted;

        /// <summary>
        /// Les permissions enlevées à tous les utilisateurs de l'application.
        /// </summary>
        public Permissions Revoked => this.revoked;

        /// <summary>
        /// Indique si l'application est utilisable ou non.
        /// </summary>
        public bool IsEnabled => this.isEnabled;

        /// <summary>
        /// Indique si les utilisateurs peuvent se connecter via l'application.
        /// </summary>
        public bool AllowUsers => this.allowUsers;

        /// <summary>
        /// Indique si des utilisateurs peuvent se connecter via l'application.
        /// </summary>
        public bool AllowUserLogin => this.AllowUsers && this.IsEnabled;

        /// <summary>
        /// Crée une application existante.
        /// </summary>
        /// <param name="apiKey">La clé d'API.</param>
        /// <param name="name">Le nom affiché de l'application.</param>
        /// <param name="isEnabled">Si l'application est active ou non.</param>
        /// <param name="granted">Les permissions accordées à tous les utilisateurs.</param>
        /// <param name="revoked">Les permissions refusées à tous les utilisateurs.</param>
        /// <param name="allowUsers">Autorise ou non les utilisateurs à se connecter via l'application.</param>
        public Client(
            string apiKey,
            string name,
            bool isEnabled,
            Permissions granted,
            Permissions revoked,
            bool allowUsers
        )
        {
            this.apiKey = apiKey;
            this.name = name;
            this.granted = granted;
            this.revoked = revoked;
            this.isEnabled = isEnabled;
            this.allowUsers = allowUsers;
        }

        /// <summary>
        /// Crée une nouvelle application.
        /// </summary>
        /// <param name="name">Le nom affiché de l'application.</param>
        /// <param name="granted">Les permissions accordées à tous les utilisateurs.</param>
        /// <param name="revoked">Les permissions refusées à tous les utilisateurs.</param>
        public Client(
            string name,
            Permissions granted = Permissions.NONE,
            Permissions revoked = Permissions.NONE,
            bool allowUsers = true
        )
        {
            var rtg = new RandomTextGenerator(new BasicRandomProvider());
            this.apiKey = rtg.AlphaNumericString(20);

            this.name = name;
            this.granted = granted;
            this.revoked = revoked;
            this.isEnabled = true;
            this.allowUsers = allowUsers;
        }

        /// <summary>
        /// Applique les filtres de permissions.
        /// Les ajouts (<see cref="Granted"/>) sont appliqués avant
        /// les restrictions (<see cref="Revoked"/>).
        /// </summary>
        /// <param name="permissions">Les permissions à filtrer.</param>
        /// <returns>Les permission restantes.</returns>
        public Permissions Filter(Permissions permissions)
        {
            return permissions.Grant(this.granted).Revoke(this.revoked);
        }
    }
}
