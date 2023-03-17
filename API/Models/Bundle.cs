using GalliumPlusAPI.Database;
using System.Text.Json.Serialization;

namespace GalliumPlusAPI.Models
{
    /// <summary>
    /// Une formule.
    /// </summary>
    public class Bundle
    {
        /// <summary>
        /// L'identifiant de la formule.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Le nom affiché de la formule.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Les identifiants des produits compris dans la formule.
        /// </summary>
        public List<int> Products { get; set; }

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
        /// Indique si le produit est disponible ou non.
        /// </summary>
        public bool Available(IDao dao) {
            switch (this.Availability)
            {
                case Availability.ALWAYS: return true;
                case Availability.AUTO: return this.Stock(dao) > 0;
                case Availability.NEVER: return false;

                // ignore les états invalides
                default: return false;
            };
        }

        /// <summary>
        /// La quantité restante en stock.
        /// </summary>
        public int Stock(IDao dao)
        {
            return this.Products.MinBy(id => dao.Products.ReadOne(id).Stock);
        }
    }
}
