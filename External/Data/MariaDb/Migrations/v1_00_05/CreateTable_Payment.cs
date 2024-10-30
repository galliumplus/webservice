using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_025)]
public class CreateTable_Payment : Migration
{
    public override void Up()
    {
        this.Create.Table("Payment")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("method").AsInt64().ForeignKey("FK_Payment_PaymentMethod_method", "PaymentMethod", "id")
            .WithColumn("amount").AsDecimal(6, 2).NotNullable()
            .WithColumn("flow").AsInt16()
            .WithColumn("date").AsDateTime2().NotNullable();
        this.Execute.Sql("ALTER TABLE `Payment` ADD CONSTRAINT ENUM_Payment_flow CHECK (`flow` BETWEEN 1 AND 2);");
    }

    public override void Down()
    {
        this.Delete.Table("Payment");
    }
}