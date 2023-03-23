using GalliumPlus.WebApi.Models;

namespace GalliumPlus.WebApi.Data
{
    public interface IBasicDao<TKey, TItem>
    where TItem : IModel<TKey>
    {
        public void Create(TItem item);

        public IEnumerable<TItem> Read();

        public TItem? Read(TKey key);

        public void Update(TKey key, TItem item);

        public void Delete(TKey key);
    }
}
