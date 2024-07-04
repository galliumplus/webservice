using FluentMigrator;

namespace GalliumPlus.WebApi.Data.MariaDb.Migrations.v1_00_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(2024_07_03_008)]
public class CreateTable_SsoClient : Migration
{
    public override void Up()
    {
        this.Create.Table("SsoClient")
            .WithColumn("id").AsInt64().PrimaryKey()
            .WithColumn("secret").AsString(30).NotNullable()
            .WithColumn("usesApi").AsBoolean().NotNullable()
            .WithColumn("redirectUrl").AsString(120).NotNullable()
            .WithColumn("logoUrl").AsString(120).Nullable();
    }

    public override void Down()
    {
        this.Delete.Table("SsoClient");
    }
}