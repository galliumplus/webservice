namespace GalliumPlus.WebApi.Models
{
    /// <summary>
    /// Une classe possédant un identifiant.
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
