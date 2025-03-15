namespace GalliumPlus.Core.Users;

public class SessionOptions
{
    /// <summary>
    /// La durée maximum d'une session utilisateur (par défaut 24 heures).
    /// </summary>
    public readonly TimeSpan LifetimeForUsers = TimeSpan.FromHours(24);
    
    /// <summary>
    /// La durée maximum d'une session applicative (par défaut 72 heures).
    /// </summary>
    public readonly TimeSpan LifetimeForApps = TimeSpan.FromHours(72);

    /// <summary>
    /// La durée maximum d'une session sans activité (par défaut 20 minutes).
    /// </summary>
    public readonly TimeSpan InactivityTimeout = TimeSpan.FromMinutes(20);
}