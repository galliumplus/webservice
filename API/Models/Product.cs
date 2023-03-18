using System.Text.Json.Serialization;
using GalliumPlusAPI.Database;

namespace GalliumPlusAPI.Models
{
    /// <summary>
    /// Un produit.
    /// </summary>
    public class Product : IModel<int>
    {
        /// <summary>
        /// L'identifiant du produit.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Le nom affiché du produit.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// La quantité restante en stock.
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// Prix non-adhérent.
        /// </summary>
        public double NonMemberPrice { get; set; }

        /// <summary>
        /// Prix adhérent.
        /// </summary>
        public double MemberPrice { get; set; }

        /// <summary>
        /// Disponibilité du produit.
        /// </summary>
        public Availability Availability { get; set; }

        /// <summary>
        /// Catégorie du produit.
        /// </summary>
        public int CategoryId { get; set; }
    
        /// <summary>
        /// Indique si le produit est disponible ou non.
        /// </summary>
        [JsonIgnore]
        public bool Available => this.Availability switch
        {
            Availability.ALWAYS => true,
            Availability.AUTO => this.Stock > 0,
            Availability.NEVER => false,

            // ignore les états invalides
            _ => false,
        };   
    }
}
