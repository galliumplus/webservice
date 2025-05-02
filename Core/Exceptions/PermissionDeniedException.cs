using GalliumPlus.Core.Security;

namespace GalliumPlus.Core.Exceptions;

/// <summary>
/// Erreur indiquant que l'utilisateur n'a pas les permissions suffisantes
/// pour effectuer une action.
/// </summary>
public class PermissionDeniedException : GalliumException
{
    private readonly Permission required;

    /// <summary>
    /// Les permissions qui étaient requises.
    /// </summary>
    public Permission Required => this.required;

    public override ErrorCode ErrorCode => ErrorCode.PermissionDenied;
        
    /// <summary>
    /// Instancie l'exception.
    /// </summary>
    /// <param name="required">Les permissions requises pour effectuer l'action.</param>
    public PermissionDeniedException(Permission required)
    {
        this.required = required;
    }
}
