using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

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
            "Mehdi Mansouri", 
            new Role(0, "Membre", Permissions.NONE),
            "Prof",
            21.30,
            false
        );

        private readonly TimeSpan timeMargin = TimeSpan.FromMilliseconds(500);

        [Fact]
        public void Constructor()
        {
            Session session = new Session("abcdef", lastUse, expiration, user);

            Assert.Equal("abcdef", session.Token);
            Assert.Equal(lastUse, session.LastUse);
            Assert.Equal(expiration, session.Expiration);
            Assert.Equal(user, session.User);
        }

        [Fact]
        public void UnusedSince()
        {
            TimeSpan fiveMinutes = TimeSpan.FromMinutes(5);
            DateTime fiveMinutesAgo = DateTime.UtcNow.Subtract(fiveMinutes);

            Session session = new Session("abcdef", fiveMinutesAgo, expiration, user);

            Assert.InRange(session.UnusedSince, fiveMinutes, fiveMinutes + timeMargin);
        }

        [Fact]
        public void ExpiresIn()
        {
            TimeSpan twoDays = TimeSpan.FromDays(2);
            DateTime twoDaysLater = DateTime.UtcNow.Add(twoDays);

            Session session = new Session("abcdef", lastUse, twoDaysLater, user);

            Assert.InRange(session.ExpiresIn, twoDays - timeMargin, twoDays);
        }

        [Fact]
        public void Expired()
        {
            DateTime recent = DateTime.UtcNow - (Session.INACTIVITY_TIMEOUT / 2);
            DateTime notRecent = DateTime.UtcNow - (Session.INACTIVITY_TIMEOUT * 2);
            DateTime future = DateTime.UtcNow.AddHours(1);
            DateTime past = DateTime.UtcNow.AddHours(-1);

            Session notExpired = new Session("abcdef", recent, future, user);
            Assert.False(notExpired.Expired);

            Session expiredInactivity = new Session("abcdef", notRecent, future, user);
            Assert.True(expiredInactivity.Expired);

            Session expiredLifetime = new Session("abcdef", recent, past, user);
            Assert.True(expiredLifetime.Expired);

            Session expiredBoth = new Session("abcdef", notRecent, past, user);
            Assert.True(expiredBoth.Expired);

            Session createdInTheFuture = new Session("abcdef", future, future, user);
            Assert.False(createdInTheFuture.Expired);
        }

        [Fact]
        public void LogIn()
        {
            DateTime now = DateTime.UtcNow;
            DateTime exp = now + Session.LIFETIME;

            Session newSession = Session.LogIn(user);

            Assert.Equal(20, newSession.Token.Length);
            Assert.InRange(newSession.LastUse, now, now + timeMargin);
            Assert.InRange(newSession.Expiration, exp, exp + timeMargin);
            Assert.Equal(user, newSession.User);
        }

        [Fact]
        public void Refresh()
        {
            DateTime recent = DateTime.UtcNow - (Session.INACTIVITY_TIMEOUT / 2);
            DateTime notRecent = DateTime.UtcNow - (Session.INACTIVITY_TIMEOUT * 2);
            DateTime future = DateTime.UtcNow.AddHours(1);
            DateTime past = DateTime.UtcNow.AddHours(-1);

            Session notExpired = new Session("abcdef", recent, future, user);
            Assert.True(notExpired.Refresh());
            Assert.InRange(notExpired.UnusedSince, TimeSpan.Zero, timeMargin);

            Session expiredInactivity = new Session("abcdef", notRecent, future, user);
            Assert.False(expiredInactivity.Refresh());

            Session expiredLifetime = new Session("abcdef", recent, past, user);
            Assert.False(expiredLifetime.Refresh());

            Session expiredBoth = new Session("abcdef", notRecent, past, user);
            Assert.False(expiredBoth.Refresh());

            Session createdInTheFuture = new Session("abcdef", future, future, user);
            Assert.True(createdInTheFuture.Refresh());
            Assert.InRange(createdInTheFuture.UnusedSince, TimeSpan.Zero, timeMargin);
        }
    }
}
