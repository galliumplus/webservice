using GalliumPlus.WebApi.Core.Users;

namespace GalliumPlus.WebApi.Core.Data
{
    public interface ISessionDao : IBasicDao<string, Session>
    {
        /// <summary>
        /// Accès au DAO des utilisateurs.
        /// </summary>
        public IUserDao Users { get; }
    }
}
