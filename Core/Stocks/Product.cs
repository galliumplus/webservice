using System.ComponentModel;
using System.Text.Json.Serialization;

namespace GalliumPlus.WebApi.Core.Stocks
{
    /// <summary>
    /// Un produit.
    /// </summary>
    public class Product
    {
        private int id;
        private string name;
        private int stock;
        private double nonMemberPrice;
        private double memberPrice;
        private Availability availability;
        private Category category;

        /// <summary>
        /// L'identifiant du produit.
        /// </summary>
        public int Id { get => this.id; set => this.id = value; }

        /// <summary>
        /// Le nom affiché du produit.
        /// </summary>
        public string Name { get => this.name; set => this.name = value; }

        /// <summary>
        /// La quantité restante en stock.
        /// </summary>
        public int Stock { get => this.stock; set => this.stock = value; }

        /// <summary>
        /// Le prix non-adhérent en euros.
        /// </summary>
        public double NonMemberPrice { get => this.nonMemberPrice; set => this.nonMemberPrice = value; }

        /// <summary>
        /// Le prix adhérent en euros.
        /// </summary>
        public double MemberPrice { get => this.memberPrice; set => this.memberPrice = value; }

        /// <summary>
        /// Disponibilité du produit.
        /// </summary>
        public Availability Availability { get => this.availability; set => this.availability = value; }

        /// <summary>
        /// Catégorie du produit.
        /// </summary>
        public Category Category { get => this.category; set => this.category = value; }

        /// <summary>
        /// Indique si le produit est disponible ou non.
        /// </summary>
        public bool Available => this.Availability switch
        {
            Availability.ALWAYS => true,
            Availability.AUTO => this.Stock > 0,
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
            Category category)
        {
            this.id = id;
            this.name = name;
            this.stock = stock;
            this.nonMemberPrice = nonMemberPrice;
            this.memberPrice = memberPrice;
            this.availability = availability;
            this.category = category;
        }

    }
}
