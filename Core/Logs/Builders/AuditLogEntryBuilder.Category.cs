using GalliumPlus.Core.Stocks;

namespace GalliumPlus.Core.Logs.Builders;

public partial class AuditLogEntryBuilder
{
    private class CategoryEntryBuilder(AuditLogEntryBuilder root, Category category)
        : GenericEntryBuilder(
            root,
            LoggedAction.CategoryAdded,
            LoggedAction.CategoryModified,
            LoggedAction.CategoryDeleted
        )
    {
        protected override void AddDetails()
        {
            this.Root.details.Add("id", category.Id);
            this.Root.details.Add("name", category.Name);
            this.Root.details.Add("type", category.Type.ToString());
        }
    }
}