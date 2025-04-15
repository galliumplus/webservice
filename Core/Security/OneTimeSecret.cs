using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Random;

namespace GalliumPlus.Core.Security;

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
        this.innerPassword = PasswordInformation.FromPassword("");
    }

    /// <summary>
    /// Re-génère le code secret.
    /// </summary>
    /// <exception cref="NotBuildException">Code généré ratée</exception>
    /// <returns>Le code secret en clair. Une fois cette valeur oubliée, le code secret ne peut plus être vu.</returns>
    public string Regenerate()
    {
        var rtg = new RandomTextGenerator(new CryptoRandomProvider());
        string newSecret = rtg.SecretKey();
        this.innerPassword = PasswordInformation.FromPassword(newSecret);
        if (newSecret == null)
            throw new NotBuildException("Le code n'a pas été généré");
        return newSecret;
    }
}