using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_026)]
public class CreateTable_OperationPayment : Migration
{
    public override void Up()
    {
        this.Create.Table("OperationPayment")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("operation").AsInt64().ForeignKey("FK_OperationPayment_Operation_operation", "Operation", "id")
            .WithColumn("payment").AsInt64().ForeignKey("FK_OperationPayment_Payment_payment", "Payment", "id")
            .WithColumn("amount").AsDecimal(6, 2).NotNullable();
    }

    public override void Down()
    {
        this.Delete.Table("OperationPayment");
    }
}