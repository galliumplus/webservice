namespace GalliumPlus.Core.Exceptions;

/// <summary>
/// Erreur indiquant des données non valides.
/// </summary>
public class InvalidResourceException : GalliumException
{
    public override ErrorCode ErrorCode => ErrorCode.InvalidResource;

    public InvalidResourceException(string message) : base(message) { }
}