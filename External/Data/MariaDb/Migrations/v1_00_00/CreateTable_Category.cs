using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_00_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_00_001)]
public class CreateTable_Category : Migration
{
    public override void Up()
    {
        this.Create.Table("Category")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("name").AsString().NotNullable();
    }

    public override void Down()
    {
        this.Delete.Table("Category");
    }
}
