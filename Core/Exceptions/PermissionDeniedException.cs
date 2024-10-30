using GalliumPlus.Core.Users;

namespace GalliumPlus.Core.Exceptions;

/// <summary>
/// Erreur indiquant que l'utilisateur n'a pas les permissions suffisantes
/// pour effectuer une action.
/// </summary>
public class PermissionDeniedException : GalliumException
{
    private Permissions required;

    /// <summary>
    /// Les permissions qu étaient requises.
    /// </summary>
    public Permissions Required { get => this.required; }

    public override ErrorCode ErrorCode => ErrorCode.PermissionDenied;
        
    /// <summary>
    /// Instancie l'exception.
    /// </summary>
    /// <param name="required">Les permissions requises pour effectuer l'action.</param>
    public PermissionDeniedException(Permissions required)
    {
        this.required = required;
    }
}
