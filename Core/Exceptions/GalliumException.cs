namespace GalliumPlus.Core.Exceptions;

public abstract class GalliumException : Exception
{
    /// <summary>
    /// Code de l'erreur levée.
    /// </summary>
    public abstract ErrorCode ErrorCode { get; }

    /// <summary>
    /// Constructeur d'une exception Gallium sans message.
    /// </summary>
    protected GalliumException() { }

    /// <summary>
    /// Constructeur d'une exception Gallium avec un message.
    /// </summary>
    /// <param name="message">String décrivant la raison.</param>
    protected GalliumException(string message) : base(message) { }
}