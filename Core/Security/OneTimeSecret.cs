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
    /// Le code secret haché.
    /// </summary>
    /// <exception cref="BlankSecretException">
    /// Si cette propriété est utilisée avant le premier appel à <see cref="Regenerate"/>.
    /// </exception>
    public byte[] Hash => this.innerPassword != null ? this.innerPassword.Hash : throw new BlankSecretException();

    /// <summary>
    /// Le sel concaténé au mot de passe avant hachage.
    /// </summary>
    /// <exception cref="BlankSecretException">
    /// Si cette propriété est utilisée avant le premier appel à <see cref="Regenerate"/>.
    /// </exception>
    public string Salt => this.innerPassword != null ? this.innerPassword.Salt : throw new BlankSecretException();

    /// <summary>
    /// Fais correspondre le code avec le mot de passe fourni
    /// </summary>
    /// <param name="otherSecret">Le code à comparer</param>
    /// <returns><c>true</c> si les codes correspondent, sinon <c>false</c></returns>
    /// <exception cref="BlankSecretException">
    /// Si cette propriété est utilisée avant le premier appel à <see cref="Regenerate"/>.
    /// </exception>
    public bool Match(string otherSecret)
    {
        if (this.innerPassword == null) throw new BlankSecretException();
        return this.innerPassword.Match(otherSecret);
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