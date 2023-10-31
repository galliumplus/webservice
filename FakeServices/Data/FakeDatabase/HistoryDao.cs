using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.History;
using System.Security.Cryptography;

namespace GalliumPlus.WebApi.Data.FakeDatabase
{
    public class HistoryDao : IHistoryDao
    {
        public void AddEntry(HistoryAction action)
        {
            Console.WriteLine($"[{DateTime.Now}][{action.ActionKind}] {action.Text} ({action.Actor ?? "?"} → {action.Target ?? "?"}, {action.NumericValue})");
        }

        public void CheckUserNotInHistory(string userId)
        {
            Console.WriteLine($"Ajout d'un utilisateur dans l'historique : {userId}");
        }

        public void UpdateUserId(string oldId, string newId)
        {
            Console.WriteLine($"MàJ d'un utilisateur dans l'historique : {oldId} devient {newId}");
        }
    }
}
