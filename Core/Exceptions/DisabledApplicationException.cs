namespace GalliumPlus.Core.Exceptions
{
    /// <summary>
    /// Exception levée quand un utilisateur tente de se connecter à une application qui a été désactivée.
    /// </summary>
    public class DisabledApplicationException : GalliumException
    {
        public override ErrorCode ErrorCode => ErrorCode.PermissionDenied;
        
        /// <summary>
        /// Instancie l'exception.
        /// </summary>
        /// <param name="appName">Le nom de l'application concernée.</param>
        public DisabledApplicationException(string appName) : base($"Impossible de se connecter à l'application {appName}, elle est désactivée.")
        {
        }
    }
}