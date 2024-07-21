using FluentMigrator;

namespace GalliumPlus.WebApi.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_011)]
public class CreateTable_Price : Migration
{
    public override void Up()
    {
        this.Create.Table("Price")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("price").AsDecimal(6, 2).NotNullable()
            .WithColumn("isDiscount").AsInt16().NotNullable()
            .WithColumn("effectiveDate").AsDate().NotNullable()
            .WithColumn("expirationDate").AsDate().NotNullable()
            .WithColumn("expiresUponExhaustion").AsBoolean().NotNullable()
            .WithColumn("type").AsInt64().ForeignKey("FK_Price_PricingType_type", "PricingType", "id")
            .WithColumn("item").AsInt64().ForeignKey("FK_Price_Item_item", "Item", "id");
        this.Execute.Sql("ALTER TABLE `Price` ADD CONSTRAINT ENUM_Price_isDiscount CHECK (`isDiscount` BETWEEN 0 AND 2);");
    }

    public override void Down()
    {
        this.Delete.Table("Price");
    }
}