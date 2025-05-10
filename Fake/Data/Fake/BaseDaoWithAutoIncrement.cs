namespace GalliumPlus.Data.Fake
{
    public abstract class BaseDaoWithAutoIncrement<TItem> : BaseDao<int, TItem>
    {
        private int nextInsertKey = 1;

        public override TItem Create(TItem item)
        {
            item = this.SetKey(item, this.nextInsertKey);
            this.nextInsertKey++;
            return base.Create(item);
        }
        
        public TItem Create(Func<int, TItem> factory)
        {
            TItem item = factory.Invoke(this.nextInsertKey);
            this.nextInsertKey++;
            return base.Create(item);
        }
    }
}
