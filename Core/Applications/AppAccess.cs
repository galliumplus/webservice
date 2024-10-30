using GalliumPlus.Core.Security;
using KiwiQuery.Mapped;

namespace GalliumPlus.Core.Applications;

/// <summary>
/// Représente un accès applicatif (pour un bot ou une application qui communique avec Gallium).
/// </summary>
public class AppAccess
{
    [Key]
    private readonly int id;
    private readonly OneTimeSecret secret;

    /// <summary>
    /// L'identifiant de l'application auquel le code appartient.
    /// </summary>
    public int Id => this.id;

    /// <summary>
    /// Le code secret servant à authentifier le bot.
    /// </summary>
    public OneTimeSecret Secret => this.secret;

    /// <summary>
    /// Crée un code d'accès applicatif existant.
    /// </summary>
    /// <param name="id">L'identifiant de l'application auquel le code appartient.</param>
    /// <param name="secret">Le code secret.</param>
    [PersistenceConstructor]
    public AppAccess(int id, OneTimeSecret secret)
    {
        this.id = id;
        this.secret = secret;
    }

    /// <summary>
    /// Crée un nouveau code d'accès applicatif.
    /// </summary>
    /// <param name="id">L'identifiant de l'application auquel le code appartient.</param>
    public AppAccess(int id)
    {
        this.id = id;
        this.secret = new OneTimeSecret();
    }

    public bool SecretsMatch(string plainSecret) => this.Secret.Match(plainSecret);

    public string RegenerateSecret() => this.Secret.Regenerate();
}