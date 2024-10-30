using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_016)]
public class CreateTable_Supplier : Migration
{
    public override void Up()
    {
        this.Create.Table("Supplier")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("supplierId").AsCustom("VARCHAR(20) CHARACTER SET ascii COLLATE ascii_bin").NotNullable().Unique()
            .NotNullable()
            .WithColumn("name").AsString(50).NotNullable();
    }

    public override void Down()
    {
        this.Delete.Table("Supplier");
    }
}