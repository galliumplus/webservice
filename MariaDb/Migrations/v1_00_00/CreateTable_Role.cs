using FluentMigrator;

namespace GalliumPlus.WebApi.Data.MariaDb.Migrations.v1_00_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(2024_07_03_004)]
public class CreateTable_Role : Migration
{
    public override void Up()
    {
        this.Create.Table("Role")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("name").AsString(50).NotNullable()
            .WithColumn("permissions").AsInt16().NotNullable();
    }

    public override void Down()
    {
        this.Delete.Table("Role");
    }
}