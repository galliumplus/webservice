namespace GalliumPlus.WebApi.Core.Random;

internal interface IRandomProvider
{
    /// <summary>
    /// Choisis un caractère aléatoire parmis une liste.
    /// </summary>
    /// <param name="allowed">La liste des caractères possibles.</param>
    /// <returns>Un caractère aléatoire.</returns>
    public char Pick(String allowed);
}