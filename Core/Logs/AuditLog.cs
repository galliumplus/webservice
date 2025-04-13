using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using KiwiQuery.Mapped;

namespace GalliumPlus.Core.Logs;

/// <summary>
/// Représente une entrée dans le journal d'audit. Cette classe est en lecture seule.
/// </summary>
public class AuditLog
{
    [Key]
    private readonly int id;

    private readonly LoggedAction action;
    private readonly DateTime time;
    private readonly int client;
    private readonly int? user;
    private readonly string details;

    /// <summary>
    /// L'identifiant unique de l'action.
    /// </summary>
    public int Id => this.id;

    /// <summary>
    /// Le type d'action.
    /// </summary>
    [JsonIgnore]
    public LoggedAction Action => this.action;
    
    /// <summary>
    /// Le code numérique du type d'action.
    /// </summary>
    public uint ActionCode => (uint) this.Action;
    
    /// <summary>
    /// La date et l'heure à laquelle l'action a été réalisée.
    /// </summary>
    public DateTime Time => this.time;

    /// <summary>
    /// L'identifiant de l'application depuis laquelle l'action a été effectuée.
    /// </summary>
    public int ClientId => this.client;

    /// <summary>
    /// L'identifiant de l'utilisateur qui a effectué l'action. Si il est null, c'est que l'action a été réalisée par un
    /// bot.
    /// </summary>
    public int? UserId => this.user;

    /// <summary>
    /// Les détails de l'action au format JSON.
    /// </summary>
    public JsonNode? Details => JsonNode.Parse(this.details);

    /// <summary>
    /// Crée une action.
    /// </summary>
    /// <param name="id">L'identifiant unique de l'action.</param>
    /// <param name="action">Le type d'action.</param>
    /// <param name="time">La date et l'heure à laquelle l'action a été réalisée.</param>
    /// <param name="client">L'identifiant de l'application depuis laquelle l'action a été effectuée.</param>
    /// <param name="user">L'identifiant de l'utilisateur qui a effectué l'action.</param>
    /// <param name="details">Les détails de l'action au format JSON.</param>
    [PersistenceConstructor]
    public AuditLog(int id, LoggedAction action, DateTime time, int client, int? user, string details)
    {
        this.id = id;
        this.action = action;
        this.time = time;
        this.client = client;
        this.user = user;
        this.details = details;
    }
}