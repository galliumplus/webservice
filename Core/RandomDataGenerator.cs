namespace GalliumPlus.WebApi.Core
{
    /// <summary>
    /// Classe encapsulant un RNG, utile pour générer du texte aléatoire.
    /// </summary>
    public class RandomDataGenerator
    {
        private Random rng;

        private const string ALPHANUM = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        /// <summary>
        /// Crée un nouveau <see cref="RandomDataGenerator"/> en utilisant un RNG avec une seed par défaut.
        /// </summary>
        public RandomDataGenerator()
        {
            this.rng = new Random();
        }

        /// <summary>
        /// Génère une suite de caractères alphanumérique (lettre minuscules,
        /// majuscules et chiffres) d'une longueur de <paramref name="size"/>.
        /// </summary>
        /// <param name="size">Le nombre de caractères à générer.</param>
        /// <returns>Une chaîne aléatoire.</returns>
        public string AlphaNumericString(int size)
        {
            string result = "";

            for (int i = 0; i < size; i++)
            {
                result += ALPHANUM[this.rng.Next(ALPHANUM.Length)];
            }

            return result;
        }
    }
}
