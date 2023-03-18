using GalliumPlusAPI.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GalliumPlusAPI.Database
{
    public interface IBasicDao<TKey, TItem>
    where TItem : IModel<TKey>
    {
        public void Create(TItem item);

        public IEnumerable<TItem> ReadAll();

        public TItem? ReadOne(TKey key);

        public void Update(TKey key, TItem item);

        public void Delete(TKey key);
    }
}
