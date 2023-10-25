using GalliumPlus.WebApi.Core.Applications;
using GalliumPlus.WebApi.Core.Random;

namespace GalliumPlus.WebApi.Core.Users
{
    public class PasswordResetToken
    {
        private string token;
        private OneTimeSecret secret;
        private DateTime expiration;
        private string userId;

        /// <summary>
        /// La durée maximum d'un jeton de réinitialisation (24 heures).
        /// </summary>
        public static readonly TimeSpan LIFETIME = TimeSpan.FromHours(24);

        /// <summary>
        /// L'heure actuelle. 
        /// Cette propriété sert à être sûr de toujours utiliser le temps UTC.
        /// </summary>
        private static DateTime Now => DateTime.UtcNow;

        /// <summary>
        /// Le jeton.
        /// </summary>
        public string Token => this.token;

        /// <summary>
        /// Le hash du code secret associé au jeton.
        /// </summary>
        public byte[] SecretHash => this.secret.Hash;

        /// <summary>
        /// Le sel utilisé pour hacher le jeton.
        /// </summary>
        public string SecretSalt => this.secret.Salt;

        /// <summary>
        /// Le moment auquel le jeton expirera.
        /// </summary>
        public DateTime Expiration => this.expiration;

        /// <summary>
        /// Le temps restant avant l'expiration du jeton.
        /// </summary>
        public TimeSpan ExpiresIn => this.expiration.Subtract(Now);

        /// <summary>
        /// L'utilisateur pour qui ce jeton a été créé.
        /// </summary>
        public string UserId => this.userId;

        /// <summary>
        /// Indique si le jeton est expiré ou non.
        /// </summary>
        public bool Expired => this.ExpiresIn < TimeSpan.Zero;

        /// <summary>
        /// Crée un jeton existant.
        /// </summary>
        /// <param name="token">Le jeton.</param>
        /// <param name="expiration">Le moment auquel la session expirera.</param>
        /// <param name="userId">L'utilisateur concerné par le jeton.</param>
        public PasswordResetToken(string token, OneTimeSecret secret, DateTime expiration, string userId)
        {
            this.token = token;
            this.secret = secret;
            this.expiration = expiration;
            this.userId = userId;
        }

        /// <summary>
        /// Crée un nouveau jeton.
        /// </summary>
        /// <param name="user">L'utilisateur pour qui créer le jeton.</param>
        /// <returns></returns>
        public static PasswordResetToken New(string userId)
        {
            var rtg = new RandomTextGenerator(new BasicRandomProvider());
            string token = rtg.AlphaNumericString(20);
            return new(token, new OneTimeSecret(), Now.Add(LIFETIME), userId);
        }

        /// <summary>
        /// Génère le code secret de ce jeton.
        /// </summary>
        /// <returns>Le code secret généré. Une fois cette valeur oubliée, elle ne peut plus être récupérée.</returns>
        public string GenerateSecret()
        {
            return this.secret.Regenerate();
        }

        /// <summary>
        /// Teste un code secret pour ce jeton.
        /// </summary>
        /// <param name="secret">Le code secret à tester.</param>
        /// <returns><see langword="true"/> si le code secret correspond, sinon <see langword="false"/>.</returns>
        public bool MatchesSecret(string secret)
        {
            return this.secret.Match(secret);
        }
    }
}
