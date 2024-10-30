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
    public Client FindByApiKeyWithAppAccess(string apiKey);

    /// <summary>
    /// Récupère un client utilisant le portail SSO par sa clé d'API.
    /// </summary>
    /// <param name="apiKey">La clé d'API du client recherché.</param>
    /// <returns>Le <see cref="Client"/> correspondant. Il aura forcément un paramétrage <see cref="SameSignOn"/>.</returns>
    /// <exception cref="ItemNotFoundException"/>
    public Client FindByApiKeyWithSameSignOn(string apiKey);

    /// <summary>
    /// Active l'accès applicatif d'un client.
    /// </summary>
    /// <param name="appAccess">Les nouveaux paramètres.</param>
    void CreateAppAccess(AppAccess appAccess);

    /// <summary>
    /// Mets à jour l'accès applicatif d'un client.
    /// </summary>
    /// <param name="appAccess">Les nouveaux paramètres.</param>
    void UpdateAppAccess(AppAccess appAccess);

    /// <summary>
    /// Retire l'accès applicatif d'un client.
    /// </summary>
    /// <param name="clientId">L'identifiant du client.</param>
    void DeleteAppAccess(int clientId);

    /// <summary>
    /// Ajoute un paramétrage SSO à un client.
    /// </summary>
    /// <param name="sameSignOn">Les nouveaux paramètres.</param>
    void CreateSameSignOn(SameSignOn sameSignOn);

    /// <summary>
    /// Mets à jour le paramétrage SSO d'un client.
    /// </summary>
    /// <param name="sameSignOn">Les nouveaux paramètres.</param>
    void UpdateSameSignOn(SameSignOn sameSignOn);

    /// <summary>
    /// Retire le paramétrage SSO d'un client.
    /// </summary>
    /// <param name="clientId">L'identifiant du client.</param>
    void DeleteSameSignOn(int clientId);
}
