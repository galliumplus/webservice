using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_010)]
public class CreateTable_BundleItems : Migration
{
    public override void Up()
    {
        this.Create.Table("BundleItems")
            .WithColumn("bundle").AsInt64().ForeignKey("FK_BundleItems_Item_bundle", "Item", "id")
            .WithColumn("item").AsInt64().ForeignKey("FK_BundleItems_Item_item", "Item", "id");
        this.Create.PrimaryKey().OnTable("BundleItems").Columns("bundle", "item");
    }

    public override void Down()
    {
        this.Delete.Table("BundleItems");
    }
}