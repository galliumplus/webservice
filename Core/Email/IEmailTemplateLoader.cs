namespace GalliumPlus.WebApi.Core.Email
{
    /// <summary>
    /// Un chargeur de modèles. Son rôle est de préparer des <see cref="EmailTemplate"/>.
    /// </summary>
    public interface IEmailTemplateLoader
    {
        /// <summary>
        /// Charge un modèle de mail.
        /// </summary>
        /// <param name="identifier">Une chaîne de caractère permettant d'identifier le modèle.</param>
        /// <param name="ignoreNullContent">Indique si un modèle introuvable doit être ignoré</param>
        /// <returns>Le modèle correspondant, ou un modèle vide.</returns>
        /// <remarks>
        /// Quand <paramref name="ignoreNullContent"/> est <see langword="true"/>
        /// (valeur par défaut), aucune erreur n'est levée dans le cas où le
        /// modèle est introuvable et un modèle vide sera renvoyé à la place.
        /// Passer ce paramètre à <see langword="false"/> lèvera une
        /// <see cref="FileNotFoundException"/> le cas échéant.
        /// </remarks>
        EmailTemplate LoadTemplate(string identifier, bool ignoreNullContent = true);
    }
}