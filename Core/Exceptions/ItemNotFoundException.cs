using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalliumPlus.WebApi.Core.Exceptions
{
    /// <summary>
    /// Erreur indiquant qu'une resource n'existe pas.
    /// </summary>
    public class ItemNotFoundException : GalliumException
    {
        public override ErrorCode ErrorCode => ErrorCode.ITEM_NOT_FOUND;

        /// <summary>
        /// Crée une nouvelle <see cref="ItemNotFoundException"/> indiquant
        /// précisément quel type de ressource est manquante.
        /// </summary>
        /// <param name="itemKind">Le type de ressource, précédé par un déterminant démonstratif.</param>
        /// <param name="femaleGendered">
        /// Mettre à faux pour les noms masculins et à vrai pour les noms
        /// féminins.
        /// </param>
        public ItemNotFoundException(string itemKind)
        : base($"{itemKind} n'existe pas.") { }

        /// <summary>
        /// Crée une nouvelle <see cref="ItemNotFoundException"/> générique.
        /// </summary>
        public ItemNotFoundException() : this("Cette ressource") { }
    }
}
