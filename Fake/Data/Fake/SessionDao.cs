using GalliumPlus.Core.Applications;
using GalliumPlus.Core.Data;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Users;

namespace GalliumPlus.Data.Fake
{
    public class SessionDao : BaseDao<string, Session>, ISessionDao
    {
        private IUserDao users;
        private IClientDao clients;

        public IUserDao Users => this.users;

        public IClientDao Clients => this.clients;

        
        public SessionDao(IUserDao users, IClientDao clients)
        {
            this.users = users;
            this.clients = clients;

            User lomens = this.Users.Read("lomens");
            Client testApp = this.clients.Read(1);
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
            if (!this.Items.TryAdd(session.Token, session))
            {
                throw new DuplicateItemException();
            }
        }

        public void UpdateLastUse(Session session)
        {
            if (!this.Items.ContainsKey(session.Token)) throw new ItemNotFoundException();
            this.Items[session.Token] = session;
        }

        public void Delete(Session session)
        {
            this.Delete(session.Token);
        }

        public void DeleteByClientId(int clientId)
        {
            foreach (Session session in this.Read().Where(session => session.Client.Id == clientId))
            {
                this.Delete(session.Token);
            }
        }

        protected override string GetKey(Session item) => item.Token;

        protected override void SetKey(ref Session item, string key)
        {
            throw new InvalidOperationException("A session token is read-only");
        }

        //public Session Read(string token) => this.Read(token);
    }
}
