using GalliumPlus.WebApi.Core.Data;
using GalliumPlus.WebApi.Core.Exceptions;

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

        virtual public TItem Create(TItem item)
        {
            lock (this.items)
            {
                if (!CheckConstraints(item))
                {
                    throw new InvalidItemException("Custom constraints violated");
                }

                TKey key = GetKey(item);

                if (!items.TryAdd(key, item))
                {
                    throw new DuplicateItemException();
                }

                return items[key];
            }
        }

        virtual public void Delete(TKey key)
        {
            lock (this.items)
            {
                if (!items.Remove(key))
                {
                    throw new ItemNotFoundException();
                }
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

        virtual public TItem Update(TKey key, TItem item)
        {
            lock (this.items)
            {
                if (!CheckConstraints(item))
                {
                    throw new InvalidItemException("Custom constraints violated");
                }

                if (!items.ContainsKey(key)) throw new ItemNotFoundException();

                SetKey(ref item, key);
                items[key] = item;
                return item;
            }
        }

        abstract protected TKey GetKey(TItem item);

        protected abstract void SetKey(ref TItem item, TKey key);

        virtual protected bool CheckConstraints(TItem item) => true;
    }
}
