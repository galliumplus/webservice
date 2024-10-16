namespace GalliumPlus.Core.Email;

/// <summary>
/// Un système d'envoi de mail.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Envoie un mail.
    /// </summary>
    /// <param name="recipient">L'adresse mail du destinataire.</param>
    /// <param name="subject">Le sujet du mail.</param>
    /// <param name="content">Le contenu du mail (typiquement le résultat de <see cref="EmailTemplate.Render"/>).</param>
    void Send(string recipient, string subject, string content);

    /// <inheritdoc cref="Send"/>
    Task SendAsync(string recipient, string subject, string content, CancellationToken ct = default);
}