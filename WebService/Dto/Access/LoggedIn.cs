using GalliumPlus.Core.Users;
using GalliumPlus.WebService.Dto.Legacy;

namespace GalliumPlus.WebService.Dto.Access;

public class LoggedIn
{
    public string Token { get; }
    public DateTime Expiration { get; }
    public UserDetails? User { get; }
    public uint Permissions { get; }

    public LoggedIn(string token, DateTime expiration, UserDetails? user, uint permissions)
    {
        this.Token = token;
        this.Expiration = expiration;
        this.User = user;
        this.Permissions = permissions;
    }

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