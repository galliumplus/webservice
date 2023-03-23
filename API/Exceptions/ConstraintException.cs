namespace GalliumPlusAPI.Exceptions
{
    /// <summary>
    /// Levée quand un élément n'a pas été trouvé dans la base de données.
    /// </summary>
    public class ConstraintException : Exception
    {
        public ConstraintException(string message) : base(message) { }
    }
}
