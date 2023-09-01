using GalliumPlus.WebApi.Core.Random;
using System.Security.Cryptography;
using System.Text;

namespace GalliumPlus.WebApi.Core.Application
{
    /// <summary>
    /// Code secret qui ne peut être vu que lorsqu'il est regénéré.
    /// </summary>
    public class OneTimeSecret
    {
        private byte[] hash;

        /// <summary>
        /// Le hash SHA-256 du code secret.
        /// </summary>
        public byte[] Hashed => this.hash;

        /// <summary>
        /// Crée un code depuis des données existantes.
        /// </summary>
        /// <param name="hash">Le hash de code secret.</param>
        public OneTimeSecret(byte[] hash)
        {
            this.hash = hash;
        }

        /// <summary>
        /// Crée un nouveau code secret. Avant d'être utilisé, il doit être (re)généré une fois.
        /// </summary>
        public OneTimeSecret()
        {
            this.hash = new byte[32];
        }

        private static byte[] Hash(string secret)
        {
            byte[] encodedSecret = Encoding.UTF8.GetBytes(secret);
            using SHA256 sha256 = SHA256.Create();
            return sha256.ComputeHash(encodedSecret);
        }

        /// <summary>
        /// Teste si un autre code secret correspond à celui-ci.
        /// </summary>
        /// <param name="secret">L'autre code secret en clair.</param>
        /// <returns><see langword="true"/> si les deux codes correspondent.</returns>
        public bool Match(string secret)
        {
            return hash.SequenceEqual(Hash(secret));
        }

        /// <summary>
        /// Teste si un autre code secret correspond à celui-ci.
        /// </summary>
        /// <param name="secret">L'autre code secret.</param>
        /// <returns><see langword="true"/> si les deux codes correspondent.</returns>
        public bool Match(OneTimeSecret secret)
        {
            return hash.SequenceEqual(secret.Hashed);
        }

        /// <summary>
        /// Regénère le code secret.
        /// </summary>
        /// <returns>Le code secret en clair. Une fois cette valeur oubliée, le code secret ne peut plus être vu.</returns>
        public string Regenerate()
        {
            var rtg = new RandomTextGenerator(new CryptoRandomProvider());
            string newSecret = rtg.SecretKey();
            this.hash = Hash(newSecret);
            return newSecret;
        }
    }
}
