using GalliumPlusAPI.Models;
using GalliumPlusAPI.Exceptions;

namespace GalliumPlusAPI.Database
{
    /// <summary>
    /// DAO des catégories.
    /// </summary>
    public interface ICategoryDao : IBasicDao<int, Category>
    {
    }
}
