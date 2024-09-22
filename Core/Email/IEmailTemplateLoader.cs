namespace GalliumPlus.WebApi.Core.Email;

/// <summary>
/// Un chargeur de modèles. Son rôle est de préparer des <see cref="EmailTemplate"/>.
/// </summary>
public interface IEmailTemplateLoader
{
    /// <summary>
    /// Charge un modèle de mail.
    /// </summary>
    /// <param name="identifier">Une chaîne de caractère permettant d'identifier le modèle.</param>
    EmailTemplate GetTemplate(string identifier);
}