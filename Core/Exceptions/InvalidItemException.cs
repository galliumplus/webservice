namespace GalliumPlus.WebApi.Core.Exceptions;

/// <summary>
/// Erreur indiquant des données non valides.
/// </summary>
public class InvalidItemException : GalliumException
{
    public override ErrorCode ErrorCode => ErrorCode.InvalidItem;

    public InvalidItemException(string message) : base(message) { }
}