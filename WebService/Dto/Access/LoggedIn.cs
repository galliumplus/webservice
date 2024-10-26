using GalliumPlus.Core.Users;
using GalliumPlus.WebService.Dto.Legacy;

namespace GalliumPlus.WebService.Dto.Access;

public class LoggedIn(string token, DateTime expiration, UserDetails? user, uint permissions)
{
    public string Token { get; } = token;
    public DateTime Expiration { get; } = expiration;
    public UserDetails? User { get; } = user;
    public uint Permissions { get; } = permissions;

    public LoggedIn(Session session)
        : this(
            session.Token,
            session.Expiration,
            session.User == null ? null : new UserDetails.Mapper().FromModel(session.User),
            (uint)session.Permissions
        )
    {
    }
}