using GalliumPlusAPI.Models;

namespace GalliumPlusAPI.Database.Implementations.FakeDatabase
{
    public abstract class FakeDaoWithAutoIncrement<TItem> : FakeDao<int, TItem>
    where TItem : IModel<int>
    {
        private int nextInsertKey = 0;

        protected int NextInsertKey => this.nextInsertKey;

        protected abstract void SetAutoKey(TItem item);

        public override void Create(TItem item)
        {
            this.SetAutoKey(item);
            this.nextInsertKey++;
            base.Create(item);
        }
    }
}
