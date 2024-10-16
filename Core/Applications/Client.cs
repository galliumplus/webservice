using GalliumPlus.WebApi.Core.Random;
using GalliumPlus.WebApi.Core.Users;
using KiwiQuery.Mapped;

namespace GalliumPlus.WebApi.Core.Applications;

/// <summary>
/// Représente une application depuis laquelle l'API est utilisée.
/// </summary>
public class Client
{
    private int id;
    private string apiKey;
    private string name;
    private Permissions granted;
    private Permissions revoked;
    private bool isEnabled;

    /// <summary>
    /// L'identifiant de l'application.
    /// </summary>
    public int Id { get => this.id; set => this.id = value; }

    /// <summary>
    /// La clé d'API de l'application.
    /// </summary>
    public string ApiKey { get => this.apiKey; set => this.apiKey = value; }

    /// <summary>
    /// Le nom servant à identifier rapidement l'application.
    /// </summary>
    public string Name { get => this.name; set => this.name = value; }

    /// <summary>
    /// Les permissions données à tous les utilisteurs de l'application.
    /// </summary>
    public Permissions Granted { get => this.granted; set => this.granted = value; }

    /// <summary>
    /// Les permissions enlevées à tous les utilisateurs de l'application.
    /// </summary>
    public Permissions Revoked { get => this.revoked; set => this.revoked = value; }

    /// <summary>
    /// Indique si l'application est utilisable ou non.
    /// </summary>
    public bool IsEnabled { get => this.isEnabled; set => this.isEnabled = value; }

    /// <summary>
    /// Indique si des utilisateurs peuvent se connecter via l'application.
    /// </summary>
    public virtual bool AllowUserLogin => this.IsEnabled;

    /// <summary>
    /// Crée une application existante.
    /// </summary>
    /// <param name="id">L'identifiant de l'application.</param>
    /// <param name="apiKey">La clé d'API.</param>
    /// <param name="name">Le nom affiché de l'application.</param>
    /// <param name="isEnabled">Si l'application est active ou non.</param>
    /// <param name="granted">Les permissions accordées à tous les utilisateurs.</param>
    /// <param name="revoked">Les permissions refusées à tous les utilisateurs.</param>
    [PersistenceConstructor]
    public Client(
        int id,
        string apiKey,
        string name,
        bool isEnabled,
        Permissions granted,
        Permissions revoked
    )
    {
        this.id = id;
        this.apiKey = apiKey;
        this.name = name;
        this.granted = granted;
        this.revoked = revoked;
        this.isEnabled = isEnabled;
    }

    /// <summary>
    /// Crée une nouvelle application.
    /// </summary>
    /// <param name="name">Le nom affiché de l'application.</param>
    /// <param name="isEnabled">Si l'application est active ou non.</param>
    /// <param name="granted">Les permissions accordées à tous les utilisateurs.</param>
    /// <param name="revoked">Les permissions refusées à tous les utilisateurs.</param>
    public Client(
        string name,
        bool isEnabled = true,
        Permissions granted = Permissions.NONE,
        Permissions revoked = Permissions.NONE
    )
    {
        var rtg = new RandomTextGenerator(new BasicRandomProvider());
        this.apiKey = rtg.AlphaNumericString(20);

        this.name = name;
        this.granted = granted;
        this.revoked = revoked;
        this.isEnabled = isEnabled;
    }

    /// <summary>
    /// Applique les filtres de permissions.
    /// Les ajouts (<see cref="Granted"/>) sont appliqués avant
    /// les restrictions (<see cref="Revoked"/>).
    /// </summary>
    /// <param name="permissions">Les permissions à filtrer.</param>
    /// <returns>Les permission restantes.</returns>
    public Permissions Filter(Permissions permissions)
    {
        return permissions.Grant(this.granted).Revoke(this.revoked);
    }
}