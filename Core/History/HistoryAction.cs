namespace GalliumPlus.Core.History;

/// <summary>
/// Représente une entrée de l'historique.
/// </summary>
public class HistoryAction
{
    private HistoryActionKind actionKind;
    private DateTime time;
    private string text;
    private string? actor, target;
    private decimal? numericValue;

    /// <summary>
    /// Le type d'action effectuée.
    /// </summary>
    public HistoryActionKind ActionKind => this.actionKind;

    /// <summary>
    /// La date et l'heure à laquelle l'action à été effectuée.
    /// </summary>
    public DateTime Time => this.time;

    /// <summary>
    /// Une phrase décrivant l'action effectuée.
    /// </summary>
    public string Text => this.text;

    /// <summary>
    /// Un identifiant représentant l'utilisateur ou le bot ayant effectué l'action.
    /// </summary>
    public string? Actor => this.actor;

    /// <summary>
    /// Un identifiant représentant l'utilisateur affecté par l'action, s'il y en a un.
    /// </summary>
    public string? Target => this.target;

    /// <summary>
    /// Une valeur numérique qui apparaît dans l'action et qui peut être
    /// utilisée dans des statistiques sans avoir à analyser le <see cref="Text"/>.
    /// </summary>
    public decimal? NumericValue => this.numericValue;

    /// <summary>
    /// Crée une entrée déjà présente dans l'historique.
    /// </summary>
    /// <param name="actionKind">Le type d'action effectuée.</param>
    /// <param name="time">La date et l'heure à laquelle l'action à été effectuée.</param>
    /// <param name="text">Une phrase décrivant l'action effectuée.</param>
    /// <param name="actor">Un identifiant représentant l'utilisateur ou le bot ayant effectué l'action.</param>
    /// <param name="target">Un identifiant représentant l'utilisateur affecté par l'action, s'il y en a un.</param>
    /// <param name="numericValue">Une valeur numérique qui apparaît dans l'action.</param>
    public HistoryAction(
        HistoryActionKind actionKind,
        DateTime time,
        string text,
        string? actor,
        string? target,
        decimal? numericValue
    )
    {
        this.actionKind = actionKind;
        this.time = time;
        this.text = text;
        this.actor = actor;
        this.target = target;
        this.numericValue = numericValue;
    }

    /// <summary>
    /// Crée une nouvelle action à entrer dans l'historique.
    /// Elle sera datée du moment ou ce constructeur sera appelé.
    /// </summary>
    /// <param name="actionKind">Le type d'action effectuée.</param>
    /// <param name="text">Une phrase décrivant l'action effectuée.</param>
    /// <param name="actor">Un identifiant représentant l'utilisateur ou le bot ayant effectué l'action.</param>
    /// <param name="target">Un identifiant représentant l'utilisateur affecté par l'action, s'il y en a un.</param>
    /// <param name="numericValue">Une valeur numérique qui apparaît dans l'action.</param>
    public HistoryAction(
        HistoryActionKind actionKind,
        string text,
        string? actor = null,
        string? target = null,
        decimal? numericValue = null
    )
        : this(actionKind, DateTime.UtcNow, text, actor, target, numericValue) { }
}