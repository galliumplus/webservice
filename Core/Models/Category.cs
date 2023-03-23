namespace GalliumPlus.WebApi.Models
{
    /// <summary>
    /// Une catégorie de produits.
    /// </summary>
    public class Category : IModel<int>
    {
        /// <summary>
        /// L'identifiant de la catégorie.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Le nom affiché de la catégorie.
        /// </summary>
        public string Name { get; set; }
    }
}
