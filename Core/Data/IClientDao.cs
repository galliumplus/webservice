using GalliumPlus.Core.Applications;
using GalliumPlus.Core.Exceptions;

namespace GalliumPlus.Core.Data;

public interface IClientDao : IBasicDao<int, Client>
{
    /// <summary>
    /// Récupère un client par sa clé d'API.
    /// </summary>
    /// <param name="apiKey">La clé d'API du client recherché.</param>
    /// <returns>Le <see cref="Client"/> correspondant.</returns>
    /// <exception cref="ItemNotFoundException"/>
    public Client FindByApiKey(string apiKey);

    /// <summary>
    /// Récupère un client automatisé par sa clé d'API.
    /// </summary>
    /// <param name="apiKey">La clé d'API du client recherché.</param>
    /// <returns>Le <see cref="Client"/> correspondant. Il aura forcément un <see cref="AppAccess"/>.</returns>
    /// <exception cref="ItemNotFoundException"/>
    public Client FindBotByApiKey(string apiKey);
}