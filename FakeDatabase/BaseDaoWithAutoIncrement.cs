namespace GalliumPlus.WebApi.Data.FakeDatabase
{
    public abstract class BaseDaoWithAutoIncrement<TItem> : BaseDao<int, TItem>
    {
        private int nextInsertKey = 1;

        public override TItem Create(TItem item)
        {
            SetKey(ref item, nextInsertKey);
            nextInsertKey++;
            return base.Create(item);
        }
    }
}
