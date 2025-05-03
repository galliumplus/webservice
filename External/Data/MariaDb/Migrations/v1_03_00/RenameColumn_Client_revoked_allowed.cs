using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_03_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_03_00_001)]
public class RenameColumn_Client_revoked_allowed : Migration
{
    public override void Up()
    {
        this.Rename.Table("PricingType").To("PriceList");
    }

    public override void Down()
    {
        this.Rename.Table("PriceList").To("PricingType");
    }
}