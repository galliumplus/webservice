namespace GalliumPlusAPI.Database.Criteria
{
    /// <summary>
    /// Critères de recherche de formules
    /// </summary>
    public struct BundleCriteria
    {
        /// <summary>
        /// Sélectionne uniquement les formules disponibles.
        /// </summary>
        public bool AvailableOnly { get; set; }
    }
}
