namespace GalliumPlus.WebApi.Core
{
    /// <summary>
    /// Erreur indiquant des données non valides.
    /// </summary>
    public class InvalidItemException : Exception
    {
        public InvalidItemException(string message) : base(message) { }
    }

    /// <summary>
    /// Erreur indiquant que la ressource demandée n'existe pas.
    /// </summary>
    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException() : base() { }
    }
}
