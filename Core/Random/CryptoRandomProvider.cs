using System.Security.Cryptography;

namespace GalliumPlus.WebApi.Core.Random;

/// <summary>
/// Générateur adapté à une utilisation cryptographique basé sur <see cref="RandomNumberGenerator"/>.
/// </summary>
internal class CryptoRandomProvider : IRandomProvider
{
    /// <summary>
    /// Crée un générateur de nombre aléatoire sécurisé pour la cryptographie.
    /// </summary>
    public CryptoRandomProvider() { }

    public char Pick(string allowed)
    {
        return allowed[RandomNumberGenerator.GetInt32(allowed.Length)];
    }
}