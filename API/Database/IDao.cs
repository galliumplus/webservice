namespace GalliumPlusAPI.Database
{
    /// <summary>
    /// DAO global.
    /// </summary>
    public interface IDao
    {
        /// <summary>
        /// DAO des produits.
        /// </summary>
        IProductDao Products { get; }

        /// <summary>
        /// DAO des catégories.
        /// </summary>
        ICategoryDao Categories { get; }
    }
}
