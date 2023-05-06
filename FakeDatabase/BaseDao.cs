using GalliumPlus.WebApi.Core;
using GalliumPlus.WebApi.Core.Data;

namespace GalliumPlus.WebApi.Data.FakeDatabase
{
    public abstract class BaseDao<TKey, TItem> : IBasicDao<TKey, TItem>
    where TKey : notnull
    {
        private Dictionary<TKey, TItem> items;

        public Dictionary<TKey, TItem> Items => items;

        public BaseDao()
        {
            items = new Dictionary<TKey, TItem>();
        }

        virtual public void Create(TItem item)
        {
            if (!CheckConstraints(item))
            {
                throw new InvalidItemException("Custom constraints violated");
            }

            if (!items.TryAdd(GetKey(item), item))
            {
                throw new InvalidItemException("An item with this key already exists");
            }
        }

        virtual public void Delete(TKey key)
        {
            if (!items.Remove(key))
            {
                throw new ItemNotFoundException();
            }
        }

        virtual public IEnumerable<TItem> Read()
        {
            return items.Values;
        }

        virtual public TItem Read(TKey key)
        {
            try
            {
                return items[key];
            }
            catch (KeyNotFoundException)
            {
                throw new ItemNotFoundException();
            }
        }

        virtual public void Update(TKey key, TItem item)
        {
            if (!CheckConstraints(item))
            {
                throw new InvalidItemException("Custom constraints violated");
            }

            try
            {
                items[key] = item;
            }
            catch (KeyNotFoundException)
            {
                throw new ItemNotFoundException();
            }
        }

        abstract protected TKey GetKey(TItem item);

        virtual protected bool CheckConstraints(TItem item) => true;
    }
}
