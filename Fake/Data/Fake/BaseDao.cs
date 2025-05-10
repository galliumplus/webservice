using System.Reflection;
using GalliumPlus.Core.Data;
using GalliumPlus.Core.Exceptions;

namespace GalliumPlus.Data.Fake
{
    public abstract class BaseDao<TKey, TItem> : IBasicDao<TKey, TItem>
    where TKey : notnull
    {
        private readonly Dictionary<TKey, TItem> items;

        protected Dictionary<TKey, TItem> Items => this.items;

        protected BaseDao()
        {
            this.items = new Dictionary<TKey, TItem>();
        }

        public virtual TItem Create(TItem client)
        {
            lock (this.items)
            {
                if (!this.CheckConstraints(client))
                {
                    throw new InvalidResourceException("Custom constraints violated");
                }

                TKey key = this.GetKey(client);

                if (!this.items.TryAdd(key, client))
                {
                    throw new DuplicateItemException();
                }

                return this.items[key];
            }
        }

        public virtual void Delete(TKey key)
        {
            lock (this.items)
            {
                if (!this.items.Remove(key))
                {
                    throw new ItemNotFoundException();
                }
            }
        }

        public virtual IEnumerable<TItem> Read()
        {
            return this.items.Values;
        }

        public virtual TItem Read(TKey key)
        {
            try
            {
                return this.items[key];
            }
            catch (KeyNotFoundException)
            {
                throw new ItemNotFoundException();
            }
        }

        public virtual TItem Update(TKey key, TItem item)
        {
            lock (this.items)
            {
                if (!this.CheckConstraints(item))
                {
                    throw new InvalidResourceException("Custom constraints violated");
                }

                if (this.items.TryGetValue(key, out TItem? value))
                {
                    item = this.SetKey(item, key);
                    AssignObject(value, item);
                    return item;
                }
                else
                {
                    throw new ItemNotFoundException();
                }
            }
        }

        private static void AssignObject<T>(T destination, T source)
        {
            if (destination == null || source == null) return;
            
            var fields = destination.GetType().GetFields(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            foreach (FieldInfo field in fields)
            {
                field.SetValue(destination, field.GetValue(source));
            }
        }

        protected abstract TKey GetKey(TItem item);

        protected abstract TItem SetKey(TItem item, TKey key);

        protected virtual bool CheckConstraints(TItem item) => true;
    }
}
