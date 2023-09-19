using GalliumPlus.WebApi.Core.History;

namespace GalliumPlus.WebApi.Core.Data
{
    public interface IHistoryDao
    {
        void AddEntry(HistoryAction action);

        void UpdateUserId(string oldId, string newId);
    }
}
