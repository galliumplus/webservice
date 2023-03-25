using GalliumPlusAPI.Database.Criteria;
using GalliumPlusAPI.Models;

namespace GalliumPlusAPI.Database
{
    /// <summary>
    /// DAO des formules.
    /// </summary>
    public interface IBundleDao : IBasicDao<int, Bundle>
    {
        /// <summary>
        /// Récupère toutes les formules correpondantes à certains critères.
        /// </summary>
        /// <param name="criteria">Les critères de recherche.</param>
        /// <returns>La liste des formules correspondantes.</returns>
        public IEnumerable<Bundle> FindAll(BundleCriteria criteria);
    }
}
