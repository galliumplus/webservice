namespace GalliumPlus.Core.Exceptions;

public class FailedPreconditionException : GalliumException
{
    public override ErrorCode ErrorCode => ErrorCode.FailedPrecondition;

    private FailedPreconditionException(string message) : base(message) { }
    
    public static FailedPreconditionException DepositIsNotEmpty() => new("L'acompte n'est pas vide.");

    public static FailedPreconditionException DoesntHaveAppAccess() => new(
        "Impossible de générer un jeton de connexion pour une application sans accès applicatif."
    );

    public static FailedPreconditionException DoesntHaveSameSignOn() => new(
        "Impossible de générer une clé de signature pour une application sans Same Sign-On configuré."
    );
}