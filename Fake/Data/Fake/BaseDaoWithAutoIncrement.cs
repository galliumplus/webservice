namespace GalliumPlus.Data.Fake
{
    public abstract class BaseDaoWithAutoIncrement<TItem> : BaseDao<int, TItem>
    {
        private int nextInsertKey = 1;

        public override TItem Create(TItem client)
        {
            this.SetKey(ref client, this.nextInsertKey);
            this.nextInsertKey++;
            return base.Create(client);
        }
    }
}
