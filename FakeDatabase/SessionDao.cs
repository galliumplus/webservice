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
 
            User lomens = this.Users.Read("lomens");
            this.Create(
                new Session(
                    "12345678901234567890",
                    DateTime.UtcNow,
                    new DateTime(2099, 12, 31),
                    lomens,
                    lomens.Role.Permissions
                )
            );

            User eb069420 = this.Users.Read("eb069420");
            this.Create(
                new Session(
                    "09876543210987654321",
                    DateTime.UtcNow,
                    new DateTime(2099, 12, 31),
                    eb069420,
                    eb069420.Role.Permissions
                )
            );
        }

        protected override string GetKey(Session item) => item.Token;

        protected override void SetKey(Session item, string key)
        {
            throw new InvalidOperationException("A session token is read-only");
        }
    }
}
