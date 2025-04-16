using System.Text;
using GalliumPlus.Core.Random;
using Konscious.Security.Cryptography;

namespace GalliumPlus.Core.Security;

public class PasswordInformation
{
    private readonly byte[] hash;
    private readonly string salt;

    /// <summary>
    /// Le mot de passe haché.
    /// </summary>
    public byte[] Hash => this.hash;

    /// <summary>
    /// Le sel concaténé au mot de passe avant hachage.
    /// </summary>
    public string Salt => this.salt;

    public PasswordInformation(byte[] hash, string salt)
    {
        this.hash = hash;
        this.salt = salt;
    }

    private static byte[] SaltAndHash(string password, string salt)
    {
        byte[] saltedPassword = Encoding.UTF8.GetBytes(password + salt);
        Argon2id argon2id = new(saltedPassword);
        argon2id.MemorySize = 19456;
        argon2id.Iterations = 2;
        argon2id.DegreeOfParallelism = 1;
        return argon2id.GetBytes(32);
    }

    public bool Match(string password)
    {
        return this.hash.SequenceEqual(SaltAndHash(password, this.salt));
    }

    public static PasswordInformation FromPassword(string password)
    {
        var rtg = new RandomTextGenerator(new CryptoRandomProvider());
        string salt = rtg.AlphaNumericString(32);
        return new PasswordInformation(SaltAndHash(password, salt), salt);
    }
}