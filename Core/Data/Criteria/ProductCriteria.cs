namespace GalliumPlusAPI.Database.Criteria
{
    /// <summary>
    /// Critères de recherche de produits
    /// </summary>
    public struct ProductCriteria
    {
        /// <summary>
        /// Sélectionne uniquement les produits disponibles.
        /// </summary>
        public bool AvailableOnly { get; set; }

        /// <summary>
        /// Sélectionne uniquement les produits d'une catégorie.
        /// </summary>
        public int? Category { get; set; }
    }
}
