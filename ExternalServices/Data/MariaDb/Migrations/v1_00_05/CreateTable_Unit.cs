using FluentMigrator;

namespace GalliumPlus.WebApi.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_012)]
public class CreateTable_Unit : Migration
{
    public override void Up()
    {
        this.Create.Table("Unit")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("type").AsInt16().NotNullable()
            .WithColumn("ratio").AsInt32().NotNullable()
            .WithColumn("packagingCode").AsString(1).NotNullable()
            .WithColumn("item").AsInt64().ForeignKey("FK_Unit_Item_item", "Item", "id");
        this.Execute.Sql("ALTER TABLE `Unit` ADD CONSTRAINT ENUM_Unit_type CHECK (`type` BETWEEN 1 AND 1);");
    }

    public override void Down()
    {
        this.Delete.Table("Unit");
    }
}