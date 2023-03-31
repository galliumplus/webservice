namespace GalliumPlus.WebApi.Core.Data
{
    public interface IUserDao : IBasicDao<string, User>
    {
        /// <summary>
        /// Mets à jour l'acompte d'un utilisateur.
        /// </summary>
        /// <param name="id">L'identifiant de l'utilisateur.</param>
        /// <param name="deposit">Le nouvel acompte.</param>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <exception cref="ValueException"></exception>
        public void UpdateDeposit(string id, double deposit);
    }
}
