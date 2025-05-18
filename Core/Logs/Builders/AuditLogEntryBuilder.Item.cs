using GalliumPlus.Core.Stocks;

namespace GalliumPlus.Core.Logs.Builders;

public partial class AuditLogEntryBuilder
{
    public interface IItemEntryBuilder : IGenericEntryBuilder
    {
        AuditLogEntryBuilder PictureAdded();
        
        AuditLogEntryBuilder PictureDeleted();
    }
    
    private class ItemEntryBuilder(AuditLogEntryBuilder root, Product item)
        : GenericEntryBuilder(
            root,
            LoggedAction.ItemAdded,
            LoggedAction.ItemModified,
            LoggedAction.ItemDeleted
        ), IItemEntryBuilder
    {
        protected override void AddDetails()
        {
            this.Root.details.Add("id", item.Id);
            this.Root.details.Add("name", item.Name);
        }

        public AuditLogEntryBuilder PictureAdded()
        {
            this.Root.action = LoggedAction.ItemPictureAdded;
            this.Root.details.Add("id", item.Id);
            return this.Root;
        }

        public AuditLogEntryBuilder PictureDeleted()
        {
            this.Root.action = LoggedAction.ItemPictureDeleted;
            this.Root.details.Add("id", item.Id);
            return this.Root;
        }
    }
}