using GalliumPlus.WebApi.Core.Exceptions;
using GalliumPlus.WebApi.Core.Users;

namespace GalliumPlus.WebApi.Core.Data
{
    public interface IUserDao : IBasicDao<string, User>
    {
        /// <summary>
        /// Accès au DAO des rôles.
        /// </summary>
        public IRoleDao Roles { get; }

        /// <summary>
        /// Récupère uniquement l'acompte d'un utilisateur.
        /// </summary>
        /// <param name="id">L'identifiant de l'utilisateur.</param>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>L'acompte en euros.</returns>
        public decimal? ReadDeposit(string id);

        /// <summary>
        /// Ajoute une certaine somme à l'acompte d'un utilisateur.
        /// </summary>
        /// <param name="id">L'identifiant de l'utilisateur.</param>
        /// <param name="money">La somme à ajouter.</param>
        /// <exception cref="ItemNotFoundException"></exception>
        public void AddToDeposit(string id, decimal money);
    }
}
