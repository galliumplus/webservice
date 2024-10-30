using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_014)]
public class CreateTable_Movement : Migration
{
    public override void Up()
    {
        this.Create.Table("Movement")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("item").AsInt64().ForeignKey("FK_Movement_Item_item", "Item", "id")
            .WithColumn("quantity").AsInt32().NotNullable()
            .WithColumn("direction").AsInt16().NotNullable()
            .WithColumn("bundle").AsInt64().ForeignKey("FK_Movement_Item_bundle", "Item", "id")
            .WithColumn("group").AsInt32().NotNullable()
            .WithColumn("total").AsDecimal(6, 2).NotNullable()
            .WithColumn("operation").AsInt64().ForeignKey("FK_Movement_Operation_operation", "Operation", "id");
        this.Execute.Sql("ALTER TABLE `Movement` ADD CONSTRAINT ENUM_Movement_direction CHECK (`direction` BETWEEN 1 AND 2);");
    }

    public override void Down()
    {
        this.Delete.Table("Movement");
    }
}