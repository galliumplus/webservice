using System.Text.Json.Serialization;
using GalliumPlus.Core.Random;
using GalliumPlus.Core.Users;
using KiwiQuery.Mapped;

namespace GalliumPlus.Core.Applications;

/// <summary>
/// Représente une application depuis laquelle l'API est utilisée.
/// </summary>
public class Client
{
    [PrimaryKey]
    private int id;
    private string apiKey;
    private string name;
    private Permissions granted;
    private Permissions allowed;
    private bool isEnabled;

    [HasOne("id")]
    private AppAccess? appAccess;

    [HasOne("id")]
    private SameSignOn? sameSignOn;

    /// <summary>
    /// L'identifiant de l'application.
    /// </summary>
    public int Id
    {
        get => this.id;
        set => this.id = value;
    }

    /// <summary>
    /// La clé d'API de l'application.
    /// </summary>
    public string ApiKey
    {
        get => this.apiKey;
        set => this.apiKey = value;
    }

    /// <summary>
    /// Le nom servant à identifier rapidement l'application.
    /// </summary>
    public string Name
    {
        get => this.name;
        set => this.name = value;
    }

    /// <summary>
    /// Les permissions autorisées aux utilisateurs de l'application.
    /// </summary>
    public Permissions Allowed
    {
        get => this.allowed;
        set => this.allowed = value;
    }

    /// <summary>
    /// Les permissions données à tous les utilisteurs de l'application.
    /// </summary>
    public Permissions Granted
    {
        get => this.granted;
        set => this.granted = value;
    }

    /// <summary>
    /// Indique si l'application est utilisable ou non.
    /// </summary>
    public bool IsEnabled
    {
        get => this.isEnabled;
        set => this.isEnabled = value;
    }

    /// <summary>
    /// Indique si des utilisateurs peuvent se connecter directement via l'application.
    /// </summary>
    [JsonIgnore]
    public bool AllowDirectUserLogin => this.IsEnabled && !this.HasAppAccess && !this.HasSameSignOn;

    /// <summary>
    /// La clé d'accès applicatif de ce client, ou <see langword="null"/> s'il n'en a pas.
    /// </summary>
    [JsonIgnore]
    public AppAccess? AppAccess { get => this.appAccess; set => this.appAccess = value; }

    /// <summary>
    /// Indique si une clé d'accès applicatif est présente.
    /// </summary>
    public bool HasAppAccess => this.appAccess != null;
    
    /// <summary>
    /// Le paramétrage SSO de ce client, ou <see langword="null"/> s'il ne l'utilise pas.
    /// </summary>
    public SameSignOn? SameSignOn { get => this.sameSignOn; set => this.sameSignOn = value; }
    
    /// <summary>
    /// Indique si un paramétrage SSO est présent.
    /// </summary>
    [JsonIgnore]
    public bool HasSameSignOn => this.sameSignOn != null;

    [JsonIgnore]
    public string DisplayName => this.SameSignOn?.DisplayName ?? this.Name;

    /// <summary>
    /// Crée une application existante.
    /// </summary>
    /// <param name="id">L'identifiant de l'application.</param>
    /// <param name="apiKey">La clé d'API.</param>
    /// <param name="name">Le nom affiché de l'application.</param>
    /// <param name="isEnabled">Si l'application est active ou non.</param>
    /// <param name="allowed">Les permissions autorisées à tous les utilisateurs.</param>
    /// <param name="granted">Les permissions données à tous les utilisateurs.</param>
    /// <param name="deleted">Si l'application a été supprimée ou non.</param>
    [PersistenceConstructor]
    public Client(
        int id,
        string apiKey,
        string name,
        bool isEnabled,
        Permissions allowed,
        Permissions granted
    )
    {
        this.id = id;
        this.apiKey = apiKey;
        this.name = name;
        this.granted = granted;
        this.allowed = allowed;
        this.isEnabled = isEnabled;
    }

    /// <summary>
    /// Crée une nouvelle application.
    /// </summary>
    /// <param name="name">Le nom affiché de l'application.</param>
    /// <param name="isEnabled">Si l'application est active ou non.</param>
    /// <param name="allowed">Les permissions autorisées à tous les utilisateurs.</param>
    /// <param name="granted">Les permissions données à tous les utilisateurs.</param>
    public Client(
        string name,
        bool isEnabled = true,
        Permissions allowed = Permissions.NONE,
        Permissions granted = Permissions.NONE
    )
    {
        var rtg = new RandomTextGenerator(new BasicRandomProvider());
        this.apiKey = rtg.AlphaNumericString(20);

        this.name = name;
        this.granted = granted;
        this.allowed = allowed;
        this.isEnabled = isEnabled;
    }

    /// <summary>
    /// Applique les filtres de permissions.
    /// Les ajouts (<see cref="Granted"/>) sont appliqués avant
    /// les restrictions (<see cref="Allowed"/>).
    /// </summary>
    /// <param name="permissions">Les permissions à filtrer.</param>
    /// <returns>Les permission restantes.</returns>
    public Permissions Filter(Permissions permissions)
    {
        return permissions.Grant(this.granted).Mask(this.allowed);
    }
}