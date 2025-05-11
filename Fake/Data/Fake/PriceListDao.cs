using GalliumPlus.Core.Data;
using GalliumPlus.Core.Stocks;

namespace GalliumPlus.Data.Fake
{
    public class PriceListDao : BaseDaoWithAutoIncrement<PriceList>, IPriceListDao
    {
        public PriceListDao()
        {
            this.Create(id => new PriceList(id, "NON ADHÉRENT", "Tarif test non-adhérent", false));
            this.Create(id => new PriceList(id, "ADHÉRENT", "Tarif test adhérent", true));
        }

        protected override int GetKey(PriceList item) => item.Id;

        protected override PriceList SetKey(PriceList item, int key) =>
            new(key, item.ShortName, item.LongName, item.RequiresMembership);
    }
}