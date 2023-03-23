using GalliumPlusAPI.Exceptions;
using GalliumPlusAPI.Models;
using System.Linq.Expressions;

namespace GalliumPlusAPI.Database.Implementations.FakeDatabase
{
    public abstract class FakeDao<TKey, TItem> : IBasicDao<TKey, TItem>
    where TKey : notnull
    where TItem : IModel<TKey>
    {
        private Dictionary<TKey, TItem> items;

        public Dictionary<TKey, TItem> Items => this.items;

        public FakeDao()
        {
            this.items = new Dictionary<TKey, TItem>();
        }

        virtual public void Create(TItem item)
        {
            if (!this.CheckConstraints(item))
            {
                throw new ConstraintException("Custom constraints violated");
            }

            if (!this.items.TryAdd(item.Id, item))
            {
                throw new ConstraintException("An item with this key already exists");
            }
        }

        virtual public void Delete(TKey key)
        {
            if (!this.items.Remove(key))
            {
                throw new ItemNotFoundException();
            }
        }

        virtual public IEnumerable<TItem> ReadAll()
        {
            return this.items.Values;
        }

        virtual public TItem ReadOne(TKey key)
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

        virtual public void Update(TKey key, TItem item)
        {
            if (!this.CheckConstraints(item))
            {
                throw new ConstraintException("Custom constraints violated");
            }

            try
            {
                this.items[key] = item;
            }
            catch (KeyNotFoundException)
            {
                throw new ItemNotFoundException();
            }
        }

        virtual protected bool CheckConstraints(TItem item) => true;
    }
}
