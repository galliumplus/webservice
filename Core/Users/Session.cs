using GalliumPlus.Core.Applications;
using GalliumPlus.Core.Random;

namespace GalliumPlus.Core.Users;

public class Session
{
    private readonly string token;
    private readonly DateTime expiration;
    private readonly User? user;
    private readonly Client client;
    private DateTime lastUse;

    /// <summary>
    /// L'heure actuelle. 
    /// Cette propriété sert à être sûr de toujours utiliser le temps UTC.
    /// </summary>
    private static DateTime Now => DateTime.UtcNow;

    /// <summary>
    /// Le jeton identifiant la session.
    /// </summary>
    public string Token => this.token;

    /// <summary>
    /// La dernière utilisation de la session.
    /// </summary>
    public DateTime LastUse => this.lastUse;

    /// <summary>
    /// La durée depuis la dernière utilisation de la session.
    /// </summary>
    public TimeSpan UnusedSince => Now.Subtract(this.lastUse);

    /// <summary>
    /// Le moment auquel la session expirera.
    /// </summary>
    public DateTime Expiration => this.expiration;

    /// <summary>
    /// Le temps restant avant l'expiration de la session.
    /// </summary>
    public TimeSpan ExpiresIn => this.expiration.Subtract(Now);

    /// <summary>
    /// L'utilisateur qui a ouvert cette session, ou <see langword="null"/> si c'est un bot.
    /// </summary>
    public User? User => this.user;

    /// <summary>
    /// L'application depuis laquelle cette session a été ouverte.
    /// </summary>
    public Client Client => this.client;

    /// <summary>
    /// Les permissions accordées pour cette session.
    /// </summary>
    public Permissions Permissions => this.client.Filter(this.user?.Role.Permissions ?? Permissions.NONE);

    /// <summary>
    /// Indique si la session est expirée ou non, en prenant en compte l'inactivité.
    /// </summary>
    public bool IsExpired(SessionOptions options)
    {
        return (this.user != null && this.UnusedSince > options.InactivityTimeout) || this.ExpiresIn < TimeSpan.Zero;
    }

    /// <summary>
    /// Crée une session.
    /// </summary>
    /// <param name="token">Le jeton identifiant le session.</param>
    /// <param name="lastUse">La dernière utilisation de la session.</param>
    /// <param name="expiration">Le moment auquel la session expirera.</param>
    /// <param name="user">L'utilisateur qui a ouvert cette session, ou null si c'est une session de bot.</param>
    /// <param name="client">L'application depuis laquelle la session a été ouverte.</param>
    public Session(string token, DateTime lastUse, DateTime expiration, User? user, Client client)
    {
        this.token = token;
        this.lastUse = lastUse;
        this.expiration = expiration;
        this.user = user;
        this.client = client;
    }

    /// <summary>
    /// Ouvre une nouvelle session.
    /// </summary>
    /// <param name="options">Les règles à suivre sur les durées de vie des sessions.</param>
    /// <param name="client">L'application pour laquelle ouvrir la session.</param>
    /// <param name="user">L'utilisateur pour qui ouvrir la session.</param>
    /// <returns></returns>
    public static Session LogIn(SessionOptions options, Client client, User? user = null)
    {
        var rtg = new RandomTextGenerator(new BasicRandomProvider());
        string token = rtg.AlphaNumericString(20);
        TimeSpan lifetime = user == null ? options.LifetimeForApps : options.LifetimeForUsers;
        return new Session(token, Now, Now.Add(lifetime), user, client);
    }

    /// <summary>
    /// Mets à jour l'heure d'accès de la session.
    /// </summary>
    /// <param name="options">Les règles à suivre sur les durées de vie des sessions.</param>
    /// <returns>
    /// <see langword="true"/> si la session à bien été mise à jour,
    /// ou <see langword="false"/> si la session a expiré depuis le dernier accès.
    /// </returns>
    public bool Refresh(SessionOptions options)
    {
        if (this.IsExpired(options))
        {
            return false;
        }
        else
        {
            this.lastUse = Now;
            return true;
        }
    }
}