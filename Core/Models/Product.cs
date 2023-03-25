using System.Text.Json.Serialization;

namespace GalliumPlus.WebApi.Models
{
    /// <summary>
    /// Un produit.
    /// </summary>
    public class Product
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
        /// Le prix non-adhérent.
        /// </summary>
        public double NonMemberPrice { get; set; }

        /// <summary>
        /// Le prix adhérent.
        /// </summary>
        public double MemberPrice { get; set; }

        /// <summary>
        /// La disponibilité du produit.
        /// </summary>
        public Availability Availability { get; set; }

        /// <summary>
        /// La catégorie du produit.
        /// </summary>
        public int Category { get; set; }
    
        /// <summary>
        /// Indique si le produit est disponible ou non.
        /// </summary>
        [JsonIgnore]
        public bool Available => this.Availability switch
        {
            Availability.ALWAYS => true,
            Availability.AUTO => this.Stock !=  0,
            Availability.NEVER => false,

            // ignore les états invalides
            _ => false,
        };

        /// <summary>
        /// Crée un produit.
        /// </summary>
        /// <param name="id">L'identifiant du produit.</param>
        /// <param name="name">Le nom affiché du produit.</param>
        /// <param name="stock">La quantité restante en stock.</param>
        /// <param name="nonMemberPrice">Prix non-adhérent.</param>
        /// <param name="memberPrice">Prix adhérent.</param>
        /// <param name="availability">Disponibilité du produit.</param>
        /// <param name="categoryId">Catégorie du produit.</param>
        public Product(
            int id,
            string name,
            int stock,
            double nonMemberPrice,
            double memberPrice,
            Availability availability,
            int categoryId)
        {
            this.Id = id;
            this.Name = name;
            this.Stock = stock;
            this.NonMemberPrice = nonMemberPrice;
            this.MemberPrice = memberPrice;
            this.Availability = availability;
            this.CategoryId = categoryId;
        }

    }
}
