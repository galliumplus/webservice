namespace GalliumPlus.Core.Exceptions;

public class FailedPreconditionException : GalliumException
{
    public override ErrorCode ErrorCode => ErrorCode.FailedPrecondition;

    public FailedPreconditionException(string message) : base(message) { }
}