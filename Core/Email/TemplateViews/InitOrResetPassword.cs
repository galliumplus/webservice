using Humanizer;
using System.Globalization;

namespace GalliumPlus.WebApi.Core.Email.TemplateViews;

/// <summary>
/// Le contenu dynamique d'un mail d'initialisation de compte ou de réinitialisation de mot de passe.
/// <br/>
/// (Ces mails sont envoyé à l'utilisateur lorsqu'un nouveau compte est créé ou qu'il a oublié son mot de passe)
/// </summary>
public class InitOrResetPassword
{
    /// <summary>
    /// Le lien permettant d'activer le compte/changer le mot de passe.
    /// </summary>
    public string Link { get; private init; }

    /// <summary>
    /// L'heure d'expiration du lien (<see cref="Link"/>).
    /// </summary>
    public DateTime Expiration { get; private init; }

    /// <summary>
    /// Une version humaine de l'heure d'expiration du lien (par ex. <em>« dans 2 heures »</em>).
    /// </summary>
    public string HumanExpiration => this.Expiration.Humanize(utcDate: true, culture: new CultureInfo("fr-FR"));

    /// <summary>
    /// Rassemble les données pour un nouveau mail de (ré)initialisation.
    /// </summary>
    /// <param name="link">L'URL complète de la page de saisie de mot de passe.</param>
    /// <param name="expiration">L'heure d'expiration du lien, en temps UTC.</param>
    public InitOrResetPassword(string link, DateTime expiration)
    {
        this.Link = link;
        if (expiration.Kind != DateTimeKind.Utc) throw new ArgumentException("The link expiration must be expressed in UTC.");
        this.Expiration = expiration;
    }
}