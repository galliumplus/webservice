using GalliumPlus.Core.Applications;

namespace GalliumPlus.Core.Exceptions;

public class AccessMethodNotAllowedException(string message) : GalliumException(message)
{
    public override ErrorCode ErrorCode => ErrorCode.AccessMethodNotAllowed;

    public static AccessMethodNotAllowedException Direct(Client client) =>
        new($"Vous ne pouvez pas vous connecter directement à {client.Name}.");
}