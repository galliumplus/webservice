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
                throw new DuplicateItemException();
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

            if (!items.ContainsKey(key)) throw new ItemNotFoundException();
            
            SetKey(item, key);
            items[key] = item;
        }

        abstract protected TKey GetKey(TItem item);

        protected abstract void SetKey(TItem item, TKey key);

        virtual protected bool CheckConstraints(TItem item) => true;
    }
}
