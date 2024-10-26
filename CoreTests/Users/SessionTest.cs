using GalliumPlus.Core.Applications;

namespace GalliumPlus.Core.Users;

public class SessionTest
{
    // heure à laquelle j'ai commencé à écrire cette classe
    private readonly DateTime lastUse = new(2023, 05, 13, 19, 13, 0);

    // heure à laquelle j'ai fini
    private readonly DateTime expiration = new(2023, 05, 13, 20, 29, 0);

    private readonly User user = new(
        1,
        "mmansouri",
        new UserIdentity("Mehdi", "Mansouri", "mehdi.mansouri@iut-dijon.u-bourgogne.fr", "PROF"),
        new Role(0, "Adhérent", Permissions.NONE),
        21.30m,
        false
    );

    private readonly Client client = new("Test client");

    private readonly TimeSpan timeMargin = TimeSpan.FromMilliseconds(500);

    [Fact]
    public void Constructor()
    {
        var session = new Session("abcdef", this.lastUse, this.expiration, this.user, this.client);

        Assert.Equal("abcdef", session.Token);
        Assert.Equal(this.lastUse, session.LastUse);
        Assert.Equal(this.expiration, session.Expiration);
        Assert.Equal(this.user, session.User);
        Assert.Equal(this.client, session.Client);
    }

    [Fact]
    public void PermissionsProperty()
    {
        this.user.Role.Permissions = Permissions.SEE_PRODUCTS_AND_CATEGORIES
                                     | Permissions.SEE_ALL_USERS_AND_ROLES;
        var client1 = new Client(
            name: "App 1",
            granted: Permissions.MANAGE_PRODUCTS,
            allowed: Permissions.MANAGE_PRODUCTS
        );
        Session session1 = Session.LogIn(client1, this.user);

        Assert.Equal(Permissions.MANAGE_PRODUCTS, session1.Permissions);

        this.user.Role.Permissions = Permissions.READ_LOGS;
        var client2 = new Client(
            name: "App 2",
            granted: Permissions.SEE_PRODUCTS_AND_CATEGORIES,
            allowed: Permissions.NONE // écrase la permission donnée précedemment
        );
        Session session2 = Session.LogIn(client2, this.user);

        Assert.Equal(Permissions.READ_LOGS, session2.Permissions);
    }

    [Fact]
    public void UnusedSince()
    {
        TimeSpan fiveMinutes = TimeSpan.FromMinutes(5);
        DateTime fiveMinutesAgo = DateTime.UtcNow.Subtract(fiveMinutes);

        var session = new Session("abcdef", fiveMinutesAgo, this.expiration, this.user, this.client);

        Assert.InRange(session.UnusedSince, fiveMinutes, fiveMinutes + this.timeMargin);
    }

    [Fact]
    public void ExpiresIn()
    {
        TimeSpan twoDays = TimeSpan.FromDays(2);
        DateTime twoDaysLater = DateTime.UtcNow.Add(twoDays);

        var session = new Session("abcdef", this.lastUse, twoDaysLater, this.user, this.client);

        Assert.InRange(session.ExpiresIn, twoDays - this.timeMargin, twoDays);
    }

    [Fact]
    public void Expired()
    {
        DateTime recent = DateTime.UtcNow - (Session.InactivityTimeout / 2);
        DateTime notRecent = DateTime.UtcNow - (Session.InactivityTimeout * 2);
        DateTime future = DateTime.UtcNow.AddHours(1);
        DateTime past = DateTime.UtcNow.AddHours(-1);

        var notExpired = new Session("abcdef", recent, future, this.user, this.client);
        Assert.False(notExpired.Expired);

        var expiredInactivity = new Session("abcdef", notRecent, future, this.user, this.client);
        Assert.True(expiredInactivity.Expired);

        var expiredLifetime = new Session("abcdef", recent, past, this.user, this.client);
        Assert.True(expiredLifetime.Expired);

        var expiredBoth = new Session("abcdef", notRecent, past, this.user, this.client);
        Assert.True(expiredBoth.Expired);

        var createdInTheFuture = new Session("abcdef", future, future, this.user, this.client);
        Assert.False(createdInTheFuture.Expired);
    }

    [Fact]
    public void LogIn()
    {
        DateTime now = DateTime.UtcNow;
        DateTime exp = now + Session.LifetimeForUsers;

        Session newSession = Session.LogIn(this.client, this.user);

        Assert.Equal(20, newSession.Token.Length);
        Assert.InRange(newSession.LastUse, now, now + this.timeMargin);
        Assert.InRange(newSession.Expiration, exp, exp + this.timeMargin);
        Assert.Equal(this.user, newSession.User);
        Assert.Equal(this.client, newSession.Client);
    }

    [Fact]
    public void Refresh()
    {
        DateTime recent = DateTime.UtcNow - (Session.InactivityTimeout / 2);
        DateTime notRecent = DateTime.UtcNow - (Session.InactivityTimeout * 2);
        DateTime future = DateTime.UtcNow.AddHours(1);
        DateTime past = DateTime.UtcNow.AddHours(-1);

        var notExpired = new Session("abcdef", recent, future, this.user, this.client);
        Assert.True(notExpired.Refresh());
        Assert.InRange(notExpired.UnusedSince, TimeSpan.Zero, this.timeMargin);

        var expiredInactivity = new Session("abcdef", notRecent, future, this.user, this.client);
        Assert.False(expiredInactivity.Refresh());

        var expiredLifetime = new Session("abcdef", recent, past, this.user, this.client);
        Assert.False(expiredLifetime.Refresh());

        var expiredBoth = new Session("abcdef", notRecent, past, this.user, this.client);
        Assert.False(expiredBoth.Refresh());

        var createdInTheFuture = new Session("abcdef", future, future, this.user, this.client);
        Assert.True(createdInTheFuture.Refresh());
        Assert.InRange(createdInTheFuture.UnusedSince, TimeSpan.Zero, this.timeMargin);
    }
}