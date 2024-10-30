using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_018)]
public class RenameTable_BotClient_AppAccess : Migration
{
    public override void Up()
    {
        this.Rename.Table("BotClient").To("AppAccess");
    }

    public override void Down()
    {
        this.Rename.Table("AppAccess").To("BotClient");
    }
}