namespace GalliumPlus.Data.Fake
{
    public abstract class BaseDaoWithAutoIncrement<TItem> : BaseDao<int, TItem>
    {
        private int nextInsertKey = 1;

        public override TItem Create(TItem item)
        {
            this.SetKey(ref item, this.nextInsertKey);
            this.nextInsertKey++;
            return base.Create(item);
        }
    }
}
