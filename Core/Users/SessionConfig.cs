namespace GalliumPlus.Core.Users;

public class SessionConfig
{
    /// <summary>
    /// La durée maximum d'une session utilisateur (24 heures).
    /// </summary>
    public readonly TimeSpan LifetimeForUsers = TimeSpan.FromHours(24);
    
    /// <summary>
    /// La durée maximum d'une session applicative (72 heures).
    /// </summary>
    public readonly TimeSpan LifetimeForApps = TimeSpan.FromHours(72);

    /// <summary>
    /// La durée maximum d'une session sans activité (4 heures).
    /// </summary>
    public readonly TimeSpan InactivityTimeout = TimeSpan.FromHours(4);
}