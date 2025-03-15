namespace GalliumPlus.Core.Users;

public class SessionOptions
{
    /// <summary>
    /// La durée maximum d'une session utilisateur (par défaut 24 heures).
    /// </summary>
    public TimeSpan LifetimeForUsers { get; set; } = TimeSpan.FromHours(24);
    
    /// <summary>
    /// La durée maximum d'une session applicative (par défaut 72 heures).
    /// </summary>
    public TimeSpan LifetimeForApps { get; set; } = TimeSpan.FromHours(72);

    /// <summary>
    /// La durée maximum d'une session sans activité (par défaut 20 minutes).
    /// </summary>
    public TimeSpan InactivityTimeout { get; set; } = TimeSpan.FromMinutes(20);
}