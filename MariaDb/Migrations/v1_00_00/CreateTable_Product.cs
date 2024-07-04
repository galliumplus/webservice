using FluentMigrator;

namespace GalliumPlus.WebApi.Data.MariaDb.Migrations.v1_00_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(2024_07_03_009)]
public class CreateTable_Product : Migration
{
    public override void Up()
    {
        this.Create.Table("Product")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("name").AsString(50).NotNullable()
            .WithColumn("stock").AsInt32().NotNullable()
            .WithColumn("nonMemberPrice").AsDecimal(6, 2).NotNullable()
            .WithColumn("memberPrice").AsDecimal(6, 2).NotNullable()
            .WithColumn("availability").AsInt16().NotNullable()
            .WithColumn("category").AsInt64().NotNullable();
    }

    public override void Down()
    {
        this.Delete.Table("Product");
    }
}