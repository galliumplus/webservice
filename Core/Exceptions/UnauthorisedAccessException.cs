namespace GalliumPlus.Core.Exceptions;

/// <summary>
/// Exception de base indiquant qu'une action ne peut pas être effectuée,
/// car le niveau d'autorisation est insuffisant (identification ou permissions manquantes par exemple).
/// </summary>
public abstract class UnauthorisedAccessException : GalliumException
{
    protected UnauthorisedAccessException()
    {
    }
    
    protected UnauthorisedAccessException(string message) : base(message)
    {
    }
}