namespace GalliumPlus.Core.Exceptions;

public class CantDeleteException : GalliumException
{
    public override ErrorCode ErrorCode => ErrorCode.CantDelete;

    public CantDeleteException(string message) : base(message)
    {
        
    }
}