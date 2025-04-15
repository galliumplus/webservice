namespace GalliumPlus.Core.Exceptions;

/// <summary>
/// Exception censée être levée des informations pour s'authentifier sont fausses ou vide (car vide c'est faux aussi)
/// </summary>
public class BadOrEmptyCredentials : Exception
{
    public BadOrEmptyCredentials(string message) : base(message) { }
}