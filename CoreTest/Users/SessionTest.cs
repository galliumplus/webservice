namespace CoreTest.Users
{
    public class SessionTest
    {
        // heure à laquelle j'ai commencé à écrire cette classe
        private readonly DateTime lastUse = new DateTime(2023, 05, 13, 19, 13, 0);

        // heure à laquelle j'ai fini
        private readonly DateTime expiration = new DateTime(2023, 05, 13, 20, 29, 0);

        private readonly User user = new User(
            "mmansouri",
            new UserIdentity("Mehdi", "Mansouri", "mehdi.mansouri@iut-dijon.u-bourgogne.fr", "PROF"),
            new Role(0, "Adhérent", Permissions.NONE),
            21.30m,
            false
        );

        private readonly Client client = new Client("Test client");

        private readonly TimeSpan timeMargin = TimeSpan.FromMilliseconds(500);

        [Fact]
        public void Constructor()
        {
            Session session = new Session("abcdef", lastUse, expiration, user, client);

            Assert.Equal("abcdef", session.Token);
            Assert.Equal(lastUse, session.LastUse);
            Assert.Equal(expiration, session.Expiration);
            Assert.Equal(user, session.User);
            Assert.Equal(client, session.Client);
        }

        [Fact]
        public void PermissionsProperty()
        {
            user.Role.Permissions = Permissions.SEE_PRODUCTS_AND_CATEGORIES
                                  | Permissions.SEE_ALL_USERS_AND_ROLES;
            Client client1 = new Client(
                name: "App 1",
                granted: Permissions.MANAGE_PRODUCTS,
                revoked: Permissions.SEE_ALL_USERS_AND_ROLES
            );
            Session session1 = Session.LogIn(client1, user);

            Assert.Equal(Permissions.MANAGE_PRODUCTS, session1.Permissions);

            user.Role.Permissions = Permissions.READ_LOGS;
            Client client2 = new Client(
                name: "App 2",
                granted: Permissions.SEE_PRODUCTS_AND_CATEGORIES,
                revoked: Permissions.MANAGE_PRODUCTS // écrase la permission donnée précedemment
            );
            Session session2 = Session.LogIn(client2, user);

            Assert.Equal(Permissions.READ_LOGS, session2.Permissions);
        }

        [Fact]
        public void UnusedSince()
        {
            TimeSpan fiveMinutes = TimeSpan.FromMinutes(5);
            DateTime fiveMinutesAgo = DateTime.UtcNow.Subtract(fiveMinutes);

            Session session = new Session("abcdef", fiveMinutesAgo, expiration, user, client);

            Assert.InRange(session.UnusedSince, fiveMinutes, fiveMinutes + timeMargin);
        }

        [Fact]
        public void ExpiresIn()
        {
            TimeSpan twoDays = TimeSpan.FromDays(2);
            DateTime twoDaysLater = DateTime.UtcNow.Add(twoDays);

            Session session = new Session("abcdef", lastUse, twoDaysLater, user, client);

            Assert.InRange(session.ExpiresIn, twoDays - timeMargin, twoDays);
        }

        [Fact]
        public void Expired()
        {
            DateTime recent = DateTime.UtcNow - (Session.INACTIVITY_TIMEOUT / 2);
            DateTime notRecent = DateTime.UtcNow - (Session.INACTIVITY_TIMEOUT * 2);
            DateTime future = DateTime.UtcNow.AddHours(1);
            DateTime past = DateTime.UtcNow.AddHours(-1);

            Session notExpired = new Session("abcdef", recent, future, user, client);
            Assert.False(notExpired.Expired);

            Session expiredInactivity = new Session("abcdef", notRecent, future, user, client);
            Assert.True(expiredInactivity.Expired);

            Session expiredLifetime = new Session("abcdef", recent, past, user, client);
            Assert.True(expiredLifetime.Expired);

            Session expiredBoth = new Session("abcdef", notRecent, past, user, client);
            Assert.True(expiredBoth.Expired);

            Session createdInTheFuture = new Session("abcdef", future, future, user, client);
            Assert.False(createdInTheFuture.Expired);
        }

        [Fact]
        public void LogIn()
        {
            DateTime now = DateTime.UtcNow;
            DateTime exp = now + Session.LIFETIME;

            Session newSession = Session.LogIn(client, user);

            Assert.Equal(20, newSession.Token.Length);
            Assert.InRange(newSession.LastUse, now, now + timeMargin);
            Assert.InRange(newSession.Expiration, exp, exp + timeMargin);
            Assert.Equal(user, newSession.User);
            Assert.Equal(client, newSession.Client);
        }

        [Fact]
        public void Refresh()
        {
            DateTime recent = DateTime.UtcNow - (Session.INACTIVITY_TIMEOUT / 2);
            DateTime notRecent = DateTime.UtcNow - (Session.INACTIVITY_TIMEOUT * 2);
            DateTime future = DateTime.UtcNow.AddHours(1);
            DateTime past = DateTime.UtcNow.AddHours(-1);

            Session notExpired = new Session("abcdef", recent, future, user, client);
            Assert.True(notExpired.Refresh());
            Assert.InRange(notExpired.UnusedSince, TimeSpan.Zero, timeMargin);

            Session expiredInactivity = new Session("abcdef", notRecent, future, user, client);
            Assert.False(expiredInactivity.Refresh());

            Session expiredLifetime = new Session("abcdef", recent, past, user, client);
            Assert.False(expiredLifetime.Refresh());

            Session expiredBoth = new Session("abcdef", notRecent, past, user, client);
            Assert.False(expiredBoth.Refresh());

            Session createdInTheFuture = new Session("abcdef", future, future, user, client);
            Assert.True(createdInTheFuture.Refresh());
            Assert.InRange(createdInTheFuture.UnusedSince, TimeSpan.Zero, timeMargin);
        }
    }
}
