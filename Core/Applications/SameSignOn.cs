using System.Text.Json.Serialization;
using GalliumPlus.Core.Random;
using GalliumPlus.Core.Users;
using KiwiQuery.Mapped;
using Multiflag;

namespace GalliumPlus.Core.Applications;

/// <summary>
/// Le paramétrage du système de <em>Same Sing-On</em> de Gallium+ pour une application.
/// </summary>
public class SameSignOn
{
    private int id;
    private string secret;
    private SignatureMethod signatureMethod;
    private SameSignOnScope scope;
    private string? displayName;
    private string redirectUrl;
    private string? logoUrl;
    
    /// <summary>
    /// L'identifiant de l'application auquel les informations appartiennent.
    /// </summary>
    [JsonIgnore]
    public int Id => this.id;

    /// <summary>
    /// Le code secret utilisé pour signer les jeton d'authentification
    /// </summary>
    [JsonIgnore]
    public string Secret => this.secret;

    /// <summary>
    /// La méthode de signature utilisée (symétrique/asymétrique).
    /// </summary>
    public SignatureMethod SignatureMethod => this.signatureMethod;

    /// <summary>
    /// La portée de l'accès aux informations des utilisateurs.
    /// </summary>
    public SameSignOnScope Scope { get => this.scope; set => this.scope = value; }
    
    /// <summary>
    /// Le nom à afficher pour présenter l'application (optionnel).
    /// </summary>
    public string? DisplayName { get => this.displayName; set => this.displayName = value; }
    
    /// <summary>
    /// L'url de redirection une fois l'authentification terminée.
    /// </summary>
    public string RedirectUrl { get => this.redirectUrl; set => this.redirectUrl = value; }

    /// <summary>
    /// L'url du logo de l'application (optionnel).
    /// </summary>
    public string? LogoUrl { get => this.logoUrl; set => this.logoUrl = value; }

    /// <summary>
    /// Si l'application a besoin ou non d'une clé d'API à la connexion.
    /// </summary>
    public bool RequiresApiKey => this.scope.Includes(SameSignOnScopes.Gallium);

    /// <summary>
    /// Crée un paramétrage SSO existant.
    /// </summary>
    /// <param name="id">L'identifiant de l'application auquel les informations appartiennent.</param>
    /// <param name="secret">Le code secret utilisé pour signer les jeton d'authentification</param>
    /// <param name="signatureMethod">La méthode de signature utilisée.</param>
    /// <param name="scope">La portée de l'accès aux informations des utilisateurs.</param>
    /// <param name="displayName">Le nom à afficher pour présenter l'application.</param>
    /// <param name="redirectUrl">L'url de redirection une fois l'authentification terminée.</param>
    /// <param name="logoUrl">L'url du logo de l'application.</param>
    [PersistenceConstructor]
    public SameSignOn(
        int id,
        string secret,
        SignatureMethod signatureMethod,
        SameSignOnScope scope,
        string? displayName,
        string redirectUrl,
        string? logoUrl
    )
    {
        this.id = id;
        this.secret = secret;
        this.signatureMethod = signatureMethod;
        this.scope = scope;
        this.displayName = displayName;
        this.redirectUrl = redirectUrl;
        this.logoUrl = logoUrl;
    }

    /// <summary>
    /// Crée un nouveau paramétrage SSO.
    /// </summary>
    /// <param name="scope">La portée de l'accès aux informations des utilisateurs.</param>
    /// <param name="displayName">Le nom à afficher pour présenter l'application.</param>
    /// <param name="redirectUrl">L'url de redirection une fois l'authentification terminée.</param>
    /// <param name="logoUrl">L'url du logo de l'application.</param>
    public SameSignOn(
        SameSignOnScope scope,
        string redirectUrl,
        string? displayName = null,
        string? logoUrl = null
    ) : this(-1, "", SignatureMethod.HS256, scope, displayName, redirectUrl, logoUrl)
    {
    }

    /// <summary>
    /// Régénère le code secret JWT pour la méthode de signature donnée.
    /// </summary>
    /// <param name="method">La méthode de signature à utiliser pour les JWT.</param>
    /// <exception cref="ArgumentException">Si la méthode n'est pas prise en charge.</exception>
    public void GenerateNewSecret(SignatureMethod method)
    {
        this.signatureMethod = method;
        
        switch (method)
        {
        case SignatureMethod.HS256:
            var rtg = new RandomTextGenerator(new CryptoRandomProvider());
            this.secret = rtg.SecretKey();
            break;

        default:
            throw new ArgumentException($"La méthode de signature {method} n'est pas prise en charge.", nameof(method));
        }
    }
}