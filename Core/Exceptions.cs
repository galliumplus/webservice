using GalliumPlus.WebApi.Core.Users;

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

    /// <summary>
    /// Erreur indiquant qu'un élément existe déjà.
    /// </summary>
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException() : base() { }
    }

    /// <summary>
    /// Erreur indiquant que l'utilisateur n'a pas les permissions suffisantes
    /// pour effectuer une action.
    /// </summary>
    public class PermissionDeniedException : Exception
    {
        private Permissions required;

        /// <summary>
        /// Les permissions qu étaient requises.
        /// </summary>
        public Permissions Required { get => this.required; }

        /// <summary>
        /// Instancie l'exception.
        /// </summary>
        /// <param name="required">Les permissions requises pour effectuer l'action.</param>
        public PermissionDeniedException(Permissions required) : base()
        {
            this.required = required;
        }
    }

    /// <summary>
    /// Erreur indiquant une vente refusée.
    /// </summary>
    public class CantSellException : Exception
    {
        public CantSellException(string message) : base(message) { }
    }
}
