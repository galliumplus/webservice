using GalliumPlus.WebApi.Core;
using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Users;

namespace GalliumPlus.WebApi.Data.FakeDatabase
{
    public class SessionDao : BaseDao<string, Session>, ISessionDao
    {
        private IUserDao users;

        public IUserDao Users => users;

        public SessionDao(IUserDao users)
        {
            this.users = users;
        }

        protected override string GetKey(Session item) => item.Token;

        protected override void SetKey(Session item, string key)
        {
            throw new InvalidOperationException("The session token is read-only");
        }
    }
}
