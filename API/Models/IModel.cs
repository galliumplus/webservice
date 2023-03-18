namespace GalliumPlusAPI.Models
{
    /// <summary>
    /// Possède un identifiant.
    /// </summary>
    /// <typeparam name="TKey">Le type de l'identifiant</typeparam>
    public interface IModel<TKey>
    {
        /// <summary>
        /// L'identifiant de l'item.
        /// </summary>
        public TKey Id { get; }
    }
}
