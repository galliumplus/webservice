using GalliumPlus.WebApi.Core.Users;

namespace GalliumPlus.WebApi.Core.Data
{
    public interface ISessionDao
    {
        /// <summary>
        /// Accès au DAO des utilisateurs.
        /// </summary>
        public IUserDao Users { get; }

        /// <summary>
        /// Accès au DAO des clients.
        /// </summary>
        public IClientDao Clients { get; }

        /// <summary>
        /// Enregistre une nouvelle session.
        /// </summary>
        /// <param name="item">La session à insérer.</param>
        public void Create(Session item);

        /// <summary>
        /// Récupère un minimum d'informations sur session correspondant à un jeton.
        /// </summary>
        /// <param name="token">Le jeton de la session à récupérer.</param>
        /// <returns>La session correspondante.</returns>
        /// <exception cref="ItemNotFoundException"></exception>
        public Session Read(string token);

        /// <summary>
        /// Mets à jour l'heure d'accès d'une session.
        /// </summary>
        /// <param name="session">La session à mettre à jour.</param>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <exception cref="InvalidItemException"></exception>
        public void UpdateLastUse(Session session);

        /// <summary>
        /// Supprimme une session.
        /// </summary>
        /// <param name="session">La session à supprimer.</param>
        public void Delete(Session session);
    }
}
