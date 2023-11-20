using GalliumPlus.WebApi.Core.Random;
using System.Security.Cryptography;
using System.Text;

namespace GalliumPlus.WebApi.Core
{
    /// <summary>
    /// Code secret qui ne peut être vu que lorsqu'il est regénéré.
    /// </summary>
    public class OneTimeSecret
    {
        private PasswordInformation innerPassword;

        public byte[] Hash => this.innerPassword.Hash;

        public string Salt => this.innerPassword.Salt;

        public bool Match(string otherSecret) => this.innerPassword.Match(otherSecret);

        /// <summary>
        /// Crée un code depuis des données existantes.
        /// </summary>
        /// <param name="hash">Le hash de code secret.</param>
        public OneTimeSecret(byte[] hash, string salt)
        {
            this.innerPassword = new(hash, salt);
        }

        /// <summary>
        /// Crée un nouveau code secret. Avant d'être utilisé, il doit être (re)généré une fois.
        /// </summary>
        public OneTimeSecret()
        {
            this.innerPassword = PasswordInformation.FromPassword("");
        }

        /// <summary>
        /// Regénère le code secret.
        /// </summary>
        /// <returns>Le code secret en clair. Une fois cette valeur oubliée, le code secret ne peut plus être vu.</returns>
        public string Regenerate()
        {
            var rtg = new RandomTextGenerator(new CryptoRandomProvider());
            string newSecret = rtg.SecretKey();
            this.innerPassword = PasswordInformation.FromPassword(newSecret);
            return newSecret;
        }
    }
}
