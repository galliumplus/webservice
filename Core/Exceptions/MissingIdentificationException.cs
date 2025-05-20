using GalliumPlus.Core.Security;

namespace GalliumPlus.Core.Exceptions;

public class MissingIdentificationException(string message) : UnauthorisedAccessException(message)
{
    public override ErrorCode ErrorCode => ErrorCode.MissingIdentification;
}