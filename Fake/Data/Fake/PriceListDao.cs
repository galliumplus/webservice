using GalliumPlus.Core.Data;
using GalliumPlus.Core.Stocks;

namespace GalliumPlus.Data.Fake
{
    public class PriceListDao : BaseDaoWithAutoIncrement<PriceList>, IPriceListDao
    {
        public PriceListDao()
        {
            this.Create(id => new PriceList(id, "Adhérent", "Tarif normal adhérent", true));
            this.Create(id => new PriceList(id, "Non-adhérent", "Tarif normal non-adhérent", false));
        }

        protected override int GetKey(PriceList item) => item.Id;

        protected override PriceList SetKey(PriceList item, int key) =>
            new(key, item.ShortName, item.LongName, item.RequiresMembership);
    }
}