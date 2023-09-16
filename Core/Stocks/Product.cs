using GalliumPlus.WebApi.Core.Exceptions;

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
        private decimal nonMemberPrice;
        private decimal memberPrice;
        private Availability availability;
        private Category category;

        /// <summary>
        /// L'identifiant du produit.
        /// </summary>
        public int Id { get => this.id; set => this.id = value; }

        /// <summary>
        /// Le nom affiché du produit.
        /// </summary>
        public string Name => this.name;

        /// <summary>
        /// La quantité restante en stock.
        /// </summary>
        public int Stock { get => this.stock; set => this.stock = value; }

        /// <summary>
        /// Le prix non-adhérent en euros.
        /// </summary>
        public decimal NonMemberPrice => this.nonMemberPrice;

        /// <summary>
        /// Le prix adhérent en euros.
        /// </summary>
        public decimal MemberPrice => this.memberPrice;

        /// <summary>
        /// Disponibilité du produit.
        /// </summary>
        public Availability Availability => this.availability;

        /// <summary>
        /// Catégorie du produit.
        /// </summary>
        public Category Category => this.category;

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

        private static decimal CheckPrice(decimal price)
        {
            decimal cents = price * 100;
            if (cents < 0)
            {
                throw new InvalidItemException("Un prix ne peux pas être négatif.");
            }
            if (cents % 1 != 0)
            {
                throw new InvalidItemException("Un prix ne peu pas avoir des fractions de centimes.");
            }
            return price;
        }

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
            decimal nonMemberPrice,
            decimal memberPrice,
            Availability availability,
            Category category)
        {
            this.id = id;
            this.name = name;
            this.stock = stock;
            this.nonMemberPrice = CheckPrice(nonMemberPrice);
            this.memberPrice = CheckPrice(memberPrice);
            this.availability = availability;
            this.category = category;
        }
    }
}
