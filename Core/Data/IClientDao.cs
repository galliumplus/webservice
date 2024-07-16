using GalliumPlus.WebApi.Core.Applications;
using GalliumPlus.WebApi.Core.Exceptions;

namespace GalliumPlus.WebApi.Core.Data
{
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
        /// <returns>Le <see cref="BotClient"/> correspondant.</returns>
        /// <exception cref="ItemNotFoundException"/>
        public BotClient FindBotByApiKey(string apiKey);

        /// <summary>
        /// Récupère un client utilisant le portail SSO par sa clé d'API.
        /// </summary>
        /// <param name="apiKey">La clé d'API du client recherché.</param>
        /// <returns>Le <see cref="SsoClient"/> correspondant.</returns>
        /// <exception cref="ItemNotFoundException"/>
        SsoClient FindSsoByApiKey(string apiKey);
    }
}
