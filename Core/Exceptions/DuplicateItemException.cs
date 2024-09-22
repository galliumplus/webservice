namespace GalliumPlus.WebApi.Core.Exceptions;

/// <summary>
/// Erreur indiquant qu'un élément existe déjà.
/// </summary>
public class DuplicateItemException : GalliumException
{
    public override ErrorCode ErrorCode => ErrorCode.DuplicateItem;

    public DuplicateItemException(string message) : base(message) { }

    public DuplicateItemException() : base("Cette ressource existe déjà.") { }
}