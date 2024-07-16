using FluentMigrator;

namespace GalliumPlus.WebApi.Data.MariaDb.Migrations.v1_00_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(2024_07_03_007)]
public class CreateTable_BotClient : Migration
{
    public override void Up()
    {
        this.Create.Table("BotClient")
            .WithColumn("id").AsInt64().PrimaryKey()
            .WithColumn("secret").AsCustom("BINARY(32)").NotNullable().WithDefaultValue(new string('\0', 32))
            .WithColumn("salt").AsCustom("CHAR(32) CHARACTER SET ascii COLLATE ascii_bin").NotNullable().WithDefaultValue("");
    }

    public override void Down()
    {
        this.Delete.Table("BotClient");
    }
}