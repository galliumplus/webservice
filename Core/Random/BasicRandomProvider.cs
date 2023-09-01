namespace GalliumPlus.WebApi.Core.Random
{
    /// <summary>
    /// Générateur simple basé sur <see cref="System.Random"/>.
    /// </summary>
    internal class BasicRandomProvider : System.Random, IRandomProvider
    {
        /// <summary>
        /// Crée un générateur de nombre aléatoire avec une seed par défaut.
        /// </summary>
        public BasicRandomProvider() : base() { }

        /// <summary>
        /// Crée un générateur de nombres alétoires avec une seed fixée.
        /// </summary>
        /// <param name="seed">La seed du générateur.</param>
        public BasicRandomProvider(int seed) : base(seed) { }

        public char Pick(string allowed)
        {
            return allowed[this.Next(allowed.Length)];
        }
    }
}
