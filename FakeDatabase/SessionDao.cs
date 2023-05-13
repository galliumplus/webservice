using GalliumPlus.WebApi.Core;
using GalliumPlus.WebApi.Core.Users;

namespace GalliumPlus.WebApi.Data.FakeDatabase
{
    public class SessionDao : BaseDao<string, Session>
    {
        protected override string GetKey(Session item) => item.Token;

        protected override void SetKey(Session item, string key)
        {
            throw new InvalidOperationException("The session token is read-only");
        }
    }
}
