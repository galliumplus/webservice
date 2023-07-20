using GalliumPlus.WebApi.Core;
using GalliumPlus.WebApi.Core.Applications;
using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Users;
using System;

namespace GalliumPlus.WebApi.Data.FakeDatabase
{
    public class BotClientDao : IBotClientDao
    {
        IClientDao clients;

        public BotClientDao(IClientDao clients)
        {
            this.clients = clients;

            BotClient testBot = new BotClient("Bot", Permissions.SEE_PRODUCTS_AND_CATEGORIES);
            testBot.ApiKey = "test-api-key-bot";
            Console.WriteLine(testBot.Secret.Regenerate());
            this.Create(testBot);
        }

        public BotClient Create(BotClient item)
        {
            return (this.clients.Create(item) as BotClient)!;
        }

        public void Delete(string key)
        {
            this.Read(key); // fais tous les checks nécessaires
            this.clients.Delete(key);
        }

        public IEnumerable<BotClient> Read()
        {
            List<BotClient> found = new();

            foreach (Client client in this.clients.Read())
            {
                if (client is BotClient bot)
                {
                    found.Add(bot);
                }
            }

            return found;
        }

        public BotClient Read(string key)
        {
            Client client = this.clients.Read(key);

            if (client is BotClient bot)
            {
                return bot;
            }
            else
            {
                throw new ItemNotFoundException();
            }
        }

        public BotClient Update(string key, BotClient item)
        {
            this.Read(key); // fais tous les checks nécessaires
            return (this.clients.Update(key, item) as BotClient)!;
        }
    }
}