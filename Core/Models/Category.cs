namespace GalliumPlus.WebApi.Models
{
    /// <summary>
    /// Une catégorie de produits.
    /// </summary>
    public class Category
    {
        /// <summary>
        /// L'identifiant de la catégorie.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Le nom affiché de la catégorie.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Crée une catégorie.
        /// </summary>
        /// <param name="id">L'identifiant de la catégorie.</param>
        /// <param name="name">Le nom affiché de la catégorie.</param>
        public Category(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

    }
}
