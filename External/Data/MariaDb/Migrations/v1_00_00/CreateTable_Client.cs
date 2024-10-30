using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_00_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_00_006)]
public class CreateTable_Client : Migration
{
    public override void Up()
    {
        this.Create.Table("Client")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("apiKey").AsCustom("CHAR(20) CHARACTER SET ascii COLLATE ascii_bin").NotNullable().Unique()
            .WithColumn("name").AsString(50).NotNullable()
            .WithColumn("granted").AsInt16().NotNullable()
            .WithColumn("revoked").AsInt16().NotNullable()
            .WithColumn("isEnabled").AsBoolean().NotNullable();
    }

    public override void Down()
    {
        this.Delete.Table("Client");
    }
}