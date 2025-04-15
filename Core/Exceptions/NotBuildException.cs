namespace GalliumPlus.Core.Exceptions;

/// <summary>
/// Exception censée être levée lorsqu'une génération a échoué ou est incomplète
/// </summary>
public class NotBuildException : GalliumException
{
    public override ErrorCode ErrorCode => ErrorCode.NotBuild; 
    
    public NotBuildException(string message) : base(message) { }
}