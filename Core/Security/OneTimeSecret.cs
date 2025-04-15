using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Random;

namespace GalliumPlus.Core.Security;

/// <summary>
/// Code secret qui ne peut être vu que lorsqu'il est regénéré.
/// </summary>
public class OneTimeSecret
{
    private PasswordInformation? innerPassword;

    /// <summary>
    /// Propriété de récupération du hachage pour le mot de passe
    /// </summary>
    /// <inheritdoc cref="PasswordInformation"/>
    /// <exception cref="BadOrEmptyCredentials">
    /// Retourne une exception format anonyme sur l'information manquante pour le mot de passe
    /// </exception>
    public byte[] Hash => this.innerPassword != null
        ? this.innerPassword.Hash
        : throw new BadOrEmptyCredentials("Une des informations pour le mot de passe n'a pas été fourni");

    /// <summary>
    /// Propriété de récupération du sel utilisé pour le hachage
    /// </summary>
    /// <inheritdoc cref="PasswordInformation"/>
    /// <exception cref="BadOrEmptyCredentials">
    /// Retourne une exception format anonyme sur l'information manquante pour le mot de passe
    /// </exception>
    public string Salt => this.innerPassword != null && !string.IsNullOrEmpty(this.innerPassword.Salt)
        ? this.innerPassword.Salt
        : throw new BadOrEmptyCredentials("Une des informations pour le mot de passe n'a pas été fourni");

    /// <summary>
    /// Fais correspondre le code avec le mot de passe fourni
    /// </summary>
    /// <param name="otherSecret">Code de comparaison</param>
    /// <returns>Un état booléen suite à la comparaison</returns>
    /// <exception cref="BadOrEmptyCredentials">Si un mot de passe n'a pas été récupéré</exception>
    public bool Match(string otherSecret)
    {
        if (this.innerPassword == null)
            throw new BadOrEmptyCredentials("Le mot de passe n'a pas été récupéré");

        bool res = this.innerPassword.Match(otherSecret);
        if (!res)
            res = false;

        return res;
    }

    /// <summary>
    /// Crée un code depuis des données existantes.
    /// </summary>
    /// <param name="hash">Le hash de code secret.</param>
    /// <param name="salt">Le sel utilisé pour le hachage.</param>
    public OneTimeSecret(byte[] hash, string salt)
    {
        this.innerPassword = new PasswordInformation(hash, salt);
    }

    /// <summary>
    /// Crée un nouveau code secret. Avant d'être utilisé, il doit être (re)généré une fois.
    /// </summary>
    public OneTimeSecret()
    {
        this.innerPassword = null;
    }

    /// <summary>
    /// Re-génère le code secret.
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