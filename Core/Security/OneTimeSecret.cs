using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Random;

namespace GalliumPlus.Core.Security;

/// <summary>
/// Code secret qui ne peut être vu que lorsqu'il est regénéré.
/// </summary>
public class OneTimeSecret
{
    private PasswordInformation innerPassword;

    /// <summary>
    /// Propriété de récupération du hachage pour le mot de passe
    /// </summary>
    /// <exception cref="BadOrEmptyCredentials"></exception>
    public byte[] Hash => this.innerPassword != null 
                          ? this.innerPassword.Hash 
                          : throw new BadOrEmptyCredentials("Une des informations pour le mot de passe n'a pas été fourni");

    /// <summary>
    /// Propriété de récupération du sel utilisé pour le hachage
    /// </summary>
    /// <exception cref="BadOrEmptyCredentials"></exception>
    public string Salt => this.innerPassword != null && !string.IsNullOrEmpty(this.innerPassword.Salt)
                          ? this.innerPassword.Salt
                          : throw new BadOrEmptyCredentials("Une des informations pour le mot de passe n'a pas été fourni");

    public bool Match(string otherSecret)
    {
        if (this.innerPassword == null)
            throw new BadOrEmptyCredentials("Le mot de passe n'a pas été récupéré");

        var res = this.innerPassword.Match(otherSecret);
        if (!res)
            throw new BadOrEmptyCredentials("Mot de passe invalide");

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
    /// <exception cref="BadOrEmptyCredentials">Code généré ratée</exception>
    /// <returns>Le code secret en clair. Une fois cette valeur oubliée, le code secret ne peut plus être vu.</returns>
    public string Regenerate()
    {
        var rtg = new RandomTextGenerator(new CryptoRandomProvider());
        string newSecret = rtg.SecretKey();
        this.innerPassword = PasswordInformation.FromPassword(newSecret);
        return newSecret;
    }
}