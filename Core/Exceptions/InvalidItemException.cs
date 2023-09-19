namespace GalliumPlus.WebApi.Core.Exceptions
{
    /// <summary>
    /// Erreur indiquant des données non valides.
    /// </summary>
    public class InvalidItemException : GalliumException
    {
        public override ErrorCode ErrorCode => ErrorCode.INVALID_ITEM;

        public InvalidItemException(string message) : base(message) { }
    }
}
