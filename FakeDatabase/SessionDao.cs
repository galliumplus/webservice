using GalliumPlus.WebApi.Core;
using GalliumPlus.WebApi.Core.Applications;
using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Users;

namespace GalliumPlus.WebApi.Data.FakeDatabase
{
    public class SessionDao : BaseDao<string, Session>, ISessionDao
    {
        private IUserDao users;
        private IClientDao clients;

        public IUserDao Users => users;

        public IClientDao Clients => clients;

        public SessionDao(IUserDao users, IClientDao clients)
        {
            this.users = users;
            this.clients = clients;

            User lomens = this.Users.Read("lomens");
            Client testApp = this.clients.Read(0);
            this.Create(
                new Session(
                    "12345678901234567890",
                    DateTime.UtcNow,
                    new DateTime(2099, 12, 31),
                    lomens,
                    testApp
                )
            );

            User eb069420 = this.Users.Read("eb069420");
            this.Create(
                new Session(
                    "09876543210987654321",
                    DateTime.UtcNow,
                    new DateTime(2099, 12, 31),
                    eb069420,
                    testApp
                )
            );
        }

        new public void Create(Session session)
        {
            if (!Items.TryAdd(session.Token, session))
            {
                throw new DuplicateItemException();
            }
        }

        public void UpdateLastUse(Session session)
        {
            if (!Items.ContainsKey(session.Token)) throw new ItemNotFoundException();
            Items[session.Token] = session;
        }

        public void Delete(Session session)
        {
            this.Delete(session.Token);
        }

        protected override string GetKey(Session item) => item.Token;

        protected override void SetKey(Session item, string key)
        {
            throw new InvalidOperationException("A session token is read-only");
        }
    }
}
