using System.Text;

namespace GalliumPlus.WebApi.Core.Random;

/// <summary>
/// Classe encapsulant un RNG, utile pour générer du texte aléatoire.
/// </summary>
internal class RandomTextGenerator
{
    private IRandomProvider rng;

    private const string ALPHANUM = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    /// <summary>
    /// Crée un nouveau <see cref="RandomTextGenerator"/> en utilisant un RNG avec une seed par défaut.
    /// </summary>
    public RandomTextGenerator(IRandomProvider provider)
    {
        rng = provider;
    }

    /// <summary>
    /// Génère une suite de caractères alphanumérique (lettre minuscules,
    /// majuscules et chiffres) d'une longueur de <paramref name="size"/>.
    /// </summary>
    /// <param name="size">Le nombre de caractères à générer.</param>
    /// <returns>Une chaîne aléatoire.</returns>
    public string AlphaNumericString(int size)
    {
        StringBuilder builder = new();

        for (int i = 0; i < size; i++)
        {
            builder.Append(rng.Pick(ALPHANUM));
        }

        return builder.ToString();
    }

    /// <summary>
    /// Génère une clé au format XXXXXXXX-XXXXXXXXXXXX-XXXXXXXX.
    /// </summary>
    /// <returns>Une clé générée aléatoirement.</returns>
    public string SecretKey()
    {
        StringBuilder builder = new();

        for (int i = 5; i < 35; i++)
        {
            if (i % 13 == 0)
            {
                builder.Append('-');
            }
            else
            {
                builder.Append(rng.Pick(ALPHANUM));
            }
        }

        return builder.ToString();
    }
}