namespace GalliumPlus.WebApi.Data.FakeDatabase
{
    public abstract class BaseDaoWithAutoIncrement<TItem> : BaseDao<int, TItem>
    {
        private int nextInsertKey = 0;

        protected abstract void SetKey(TItem item, int key);

        public override void Create(TItem item)
        {
            SetKey(item, nextInsertKey);
            nextInsertKey++;
            base.Create(item);
        }
    }
}
