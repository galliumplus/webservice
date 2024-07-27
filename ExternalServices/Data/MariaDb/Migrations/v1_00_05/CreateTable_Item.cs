using FluentMigrator;

namespace GalliumPlus.WebApi.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_008)]
public class CreateTable_Item : Migration
{
    public override void Up()
    {
        this.Create.Table("Item")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("name").AsString(50).NotNullable()
            .WithColumn("isBundle").AsBoolean().NotNullable()
            .WithColumn("isAvailable").AsInt16().NotNullable()
            .WithColumn("currentStock").AsInt32().NotNullable()
            .WithColumn("category").AsInt64().ForeignKey("FK_Item_Category_category", "Category", "id").NotNullable()
            .WithColumn("group").AsInt64().ForeignKey("FK_Item_Category_group", "Category", "id").Nullable()
            .WithColumn("picture").AsString(20).Nullable()
            .WithColumn("deleted").AsBoolean().NotNullable();
        this.Execute.Sql("ALTER TABLE `Item` ADD CONSTRAINT ENUM_Item_isAvailable CHECK (`isAvailable` BETWEEN 0 AND 2);");
        this.Execute.Sql("ALTER TABLE `Item` ADD CONSTRAINT NN_Item_currentStock CHECK (`currentStock` >= 0);");
    }

    public override void Down()
    {
        this.Delete.Table("Item");
    }
}