using GalliumPlus.Core.Stocks;

namespace GalliumPlus.Core.Logs.Builders;

public partial class AuditLogEntryBuilder
{
    private class PriceListEntryBuilder(AuditLogEntryBuilder root, PriceList priceList)
        : GenericEntryBuilder(
            root,
            LoggedAction.PriceListAdded,
            LoggedAction.PriceListModified,
            LoggedAction.PriceListDeleted
        )
    {
        protected override void AddDetails()
        {
            this.Root.details.Add("id", priceList.Id);
            this.Root.details.Add("name", priceList.LongName);
        }
    }
}