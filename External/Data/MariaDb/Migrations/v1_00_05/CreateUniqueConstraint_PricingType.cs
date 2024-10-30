using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_005)]
public class CreateUniqueConstraint_PricingType : Migration
{
    public override void Up()
    {
        this.Create.UniqueConstraint("UNI_PricingType")
            .OnTable("PricingType")
            .Columns("requiresMembership", "applicableDuring");
    }

    public override void Down()
    {
        this.Delete.UniqueConstraint("UNI_PricingType").FromTable("PricingType");
    }
}