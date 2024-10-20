using GalliumPlus.Core.Security;
using GalliumPlus.Core.Users;
using KiwiQuery.Mapped;

namespace GalliumPlus.Core.Applications;

/// <summary>
/// Représente un accès applicatif (pour un bot ou une application qui communique avec Gallium).
/// </summary>
public class AppAccess
{
    private int id;
    private OneTimeSecret secret;

    /// <summary>
    /// Le code secret servant à authentifier le bot.
    /// </summary>
    public OneTimeSecret Secret => this.secret;

    /// <summary>
    /// Crée un code d'accès applicatif existant.
    /// </summary>
    /// <param name="id">L'identifiant de l'application auquel la code appartient.</param>
    /// <param name="secret">Le code secret.</param>
    public AppAccess(int id, OneTimeSecret secret)
    {
        this.id = id;
        this.secret = secret;
    }

    /// <summary>
    /// Crée un nouveau code d'accès applicatif.
    /// </summary>
    public AppAccess()
    {
        this.secret = new OneTimeSecret();
    }
}