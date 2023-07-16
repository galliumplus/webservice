using GalliumPlus.WebApi.Core.Random;
using System.ComponentModel;

namespace GalliumPlus.WebApi.Core.Users
{
    public class Session
    {
        private string token;
        private DateTime lastUse;
        private DateTime expiration;
        private User user;
        private Permissions permissions;

        /// <summary>
        /// La durée maximum d'une session (24 heures).
        /// </summary>
        public static readonly TimeSpan LIFETIME = TimeSpan.FromHours(24);

        /// <summary>
        /// La durée d'une session sans activité (30 minutes).
        /// </summary>
        public static readonly TimeSpan INACTIVITY_TIMEOUT = TimeSpan.FromMinutes(30);

        /// <summary>
        /// L'heure actuelle. 
        /// Cette propriété sert à être sûr de toujours utiliser le temps UTC.
        /// </summary>
        private static DateTime Now => DateTime.UtcNow;

        /// <summary>
        /// Le jeton identifiant la session.
        /// </summary>
        public string Token => this.token;

        /// <summary>
        /// La dernière utilisation de la session.
        /// </summary>
        public DateTime LastUse => this.lastUse;

        /// <summary>
        /// La durée depuis la dernière utilisation de la session.
        /// </summary>
        public TimeSpan UnusedSince => Now.Subtract(this.lastUse);

        /// <summary>
        /// Le moment auquel la session expirera.
        /// </summary>
        public DateTime Expiration => this.expiration;

        /// <summary>
        /// Le temps restant avant l'expiration de la session.
        /// </summary>
        public TimeSpan ExpiresIn => this.expiration.Subtract(Now);

        /// <summary>
        /// L'utilisateur qui a ouvert cette session.
        /// </summary>
        public User User => this.user;

        /// <summary>
        /// Les permissions accordées pour cette session.
        /// </summary>
        public Permissions Permissions => this.permissions;

        /// <summary>
        /// Indique si la session a expiré ou non, en prenant en compte l'inactivité.
        /// </summary>
        public bool Expired => this.UnusedSince > INACTIVITY_TIMEOUT || this.ExpiresIn < TimeSpan.Zero;

        /// <summary>
        /// Crée une session.
        /// </summary>
        /// <param name="token">Le jeton identifiant le session.</param>
        /// <param name="lastUse">La dernière utilisation de la session.</param>
        /// <param name="expiration">Le moment auquel la session expirera.</param>
        /// <param name="user">L'utilisateur qui a ouvert cette session.</param>
        public Session(string token, DateTime lastUse, DateTime expiration, User user, Permissions permissions)
        {
            this.token = token;
            this.lastUse = lastUse;
            this.expiration = expiration;
            this.user = user;
            this.permissions = permissions;
        }

        /// <summary>
        /// Ouvre une nouvelle session.
        /// </summary>
        /// <param name="token">Le jeton à utiliser pour identifier la session.</param>
        /// <param name="user">L'utilisateur pour qui ouvrir la session.</param>
        /// <param name="permissions">Les permissions à donner à l'utilisateur durant cette session.</param>
        /// <returns></returns>
        public static Session LogIn(User user, Permissions permissions)
        {
            var rtg = new RandomTextGenerator(new BasicRandomProvider());
            string token = rtg.AlphaNumericString(20);
            return new(token, Now, Now.Add(LIFETIME), user, permissions);
        }

        /// <summary>
        /// Mets à jour l'heure d'accès de la session.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> si la session à bien été mise à jour,
        /// ou <see langword="false"/> si la session a expiré depuis le dernier accès.
        /// </returns>
        public bool Refresh()
        {
            if (this.Expired) return false;

            this.lastUse = Now;
            return true;
        }
    }
}
