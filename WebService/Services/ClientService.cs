using GalliumPlus.Core.Applications;
using GalliumPlus.Core.Data;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Security;
using GalliumPlus.WebService.Dto.Access;
using GalliumPlus.WebService.Dto.Applications;

namespace GalliumPlus.WebService.Services;

[ScopedService]
public class ClientService(IClientDao clientDao)
{
    public IEnumerable<Client> GetAll() => clientDao.Read();

    public Client GetById(int id) => clientDao.Read(id);

    public SsoClientPublicInfo GetPublicInfoByApiKey(string apiKey)
    {
        Client app = clientDao.FindByApiKeyWithSameSignOn(apiKey);
        return new SsoClientPublicInfo(
            app.DisplayName,
            app.SameSignOn!.LogoUrl,
            app.SameSignOn.Scope
        );
    }

    public Client Create(PartialClient newClient)
    {
        Client client = clientDao.Create(
            new Client(
                newClient.Name,
                newClient.IsEnabled,
                (Permission)newClient.Allowed,
                (Permission)newClient.Granted
            )
        );

        if (newClient.HasAppAccess)
        {
            client.AppAccess = new AppAccess(client.Id);
            client.AppAccess.RegenerateSecret();
            clientDao.CreateAppAccess(client.AppAccess);
        }

        if (newClient.SameSignOn != null)
        {
            client.SameSignOn = new SameSignOn(
                client.Id,
                (SameSignOnScope)newClient.SameSignOn.Scope,
                newClient.SameSignOn.RedirectUrl,
                newClient.SameSignOn.DisplayName,
                newClient.SameSignOn.LogoUrl
            );
            clientDao.CreateSameSignOn(client.SameSignOn);
        }

        return client;
    }

    public GeneratedSecret GenerateNewAppAccessSecret(Client client)
    {
        if (!client.HasAppAccess)
        {
            throw FailedPreconditionException.DoesntHaveAppAccess();
        }

        string newSecret = client.AppAccess!.RegenerateSecret();
        clientDao.UpdateAppAccess(client.AppAccess);

        return new GeneratedSecret(newSecret);
    }

    public GeneratedSecret GenerateNewSameSignOnSecret(Client client, SignatureType signatureType)
    {
        if (!client.HasSameSignOn)
        {
            throw FailedPreconditionException.DoesntHaveSameSignOn();
        }

        try
        {
            client.SameSignOn!.GenerateNewSecret(signatureType);
        }
        catch (ArgumentException)
        {
            throw new InvalidResourceException("Ce type de signature n'est pas pris en charge.");
        }

        clientDao.UpdateSameSignOn(client.SameSignOn);

        return new GeneratedSecret(client.SameSignOn.Secret, client.SameSignOn.SignatureType);
    }

    public Client Update(int id, PartialClient clientUpdate)
    {
        Client client = clientDao.Read(id);

        client.Name = clientUpdate.Name;
        client.Allowed = (Permission)clientUpdate.Allowed;
        client.Granted = (Permission)clientUpdate.Granted;
        client.IsEnabled = clientUpdate.IsEnabled;
        clientDao.Update(id, client);

        if (clientUpdate.HasAppAccess && !client.HasAppAccess)
        {
            client.AppAccess = new AppAccess(client.Id);
            client.AppAccess.RegenerateSecret();
            clientDao.CreateAppAccess(client.AppAccess);
        }
        else if (!clientUpdate.HasAppAccess && client.HasAppAccess)
        {
            clientDao.DeleteAppAccess(id);
        }

        if (clientUpdate.SameSignOn != null)
        {
            if (!client.HasSameSignOn)
            {
                client.SameSignOn = new SameSignOn(
                    id,
                    (SameSignOnScope)clientUpdate.SameSignOn.Scope,
                    clientUpdate.SameSignOn.RedirectUrl,
                    clientUpdate.SameSignOn.DisplayName,
                    clientUpdate.SameSignOn.LogoUrl
                );
                clientDao.CreateSameSignOn(client.SameSignOn);
            }
            else
            {
                client.SameSignOn!.Scope = (SameSignOnScope)clientUpdate.SameSignOn.Scope;
                client.SameSignOn!.RedirectUrl = clientUpdate.SameSignOn.RedirectUrl;
                client.SameSignOn!.DisplayName = clientUpdate.SameSignOn.DisplayName;
                client.SameSignOn!.LogoUrl = clientUpdate.SameSignOn.LogoUrl;
                clientDao.UpdateSameSignOn(client.SameSignOn);
            }
        }
        else if (client.HasSameSignOn)
        {
            clientDao.DeleteSameSignOn(id);
        }

        return client;
    }

    public Client Delete(int id)
    {
        Client client = clientDao.Read(id);
        clientDao.Delete(id);
        return client;
    }
}