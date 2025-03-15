using System.Reflection;
using GalliumPlus.Core.Data;
using GalliumPlus.Core.Exceptions;

namespace GalliumPlus.Data.Fake
{
    public abstract class BaseDao<TKey, TItem> : IBasicDao<TKey, TItem>
    where TKey : notnull
    {
        private Dictionary<TKey, TItem> items;

        public Dictionary<TKey, TItem> Items => this.items;

        public BaseDao()
        {
            this.items = new Dictionary<TKey, TItem>();
        }

        virtual public TItem Create(TItem client)
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

        virtual public void Delete(TKey key)
        {
            lock (this.items)
            {
                if (!this.items.Remove(key))
                {
                    throw new ItemNotFoundException();
                }
            }
        }

        virtual public IEnumerable<TItem> Read()
        {
            return this.items.Values;
        }

        virtual public TItem Read(TKey key)
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

        virtual public TItem Update(TKey key, TItem item)
        {
            lock (this.items)
            {
                if (!this.CheckConstraints(item))
                {
                    throw new InvalidResourceException("Custom constraints violated");
                }

                if (!this.items.ContainsKey(key)) throw new ItemNotFoundException();

                this.SetKey(ref item, key);
                AssignObject(this.items[key], item);
                return item;
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

        abstract protected TKey GetKey(TItem item);

        protected abstract void SetKey(ref TItem item, TKey key);

        virtual protected bool CheckConstraints(TItem item) => true;
    }
}
