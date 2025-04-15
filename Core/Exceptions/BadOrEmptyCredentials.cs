namespace GalliumPlus.Core.Exceptions;

/// <summary>
/// Exception censée être levée des informations pour s'authentifier sont fausses ou vide (car vide c'est faux aussi)
/// </summary>
public class BadOrEmptyCredentials : GalliumException
{
    public override ErrorCode ErrorCode => ErrorCode.NotBuild; 
    
    public BadOrEmptyCredentials(string message) : base(message) { }
}