using FluentMigrator;

namespace GalliumPlus.WebApi.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_004)]
public class CreateTable_PricingType : Migration
{
    public override void Up()
    {
        this.Create.Table("PricingType")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("name").AsString(50).NotNullable()
            .WithColumn("requiresMembership").AsBoolean().NotNullable()
            .WithColumn("applicableDuring").AsInt64()
            .ForeignKey("FK_PricingType_Event_applicableDuring", "Event", "id");

        /*
           .WithColumn("isAvailable").AsInt16().NotNullable()
           .WithColumn("isDiscount").AsInt16().NotNullable()
           .WithColumn("effectiveDate").AsDate().NotNullable()
           .WithColumn("expirationDate").AsDate().NotNullable()
           .WithColumn("expiresUponExhaustion").AsBoolean().NotNullable()
           .WithColumn("extends").AsInt64().ForeignKey("FK_PricingType_PricingType_extends", "PricingType", "id").Nullable();
           */
    }

    public override void Down()
    {
        this.Delete.Table("PricingType");
    }
}