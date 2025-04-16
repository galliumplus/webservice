namespace GalliumPlus.Core.Exceptions;

/// <summary>
/// Exception levée si on tente d'utiliser un OneTimeSecret sans l'avoir généré au préalable.
/// </summary>
public class BlankSecretException()
    : Exception("Regenerate() doit être appelée au moins une fois avant d'utiliser un OneTimeSecret");