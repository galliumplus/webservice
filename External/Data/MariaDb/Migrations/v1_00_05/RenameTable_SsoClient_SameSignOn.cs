using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_019)]
public class RenameTable_SsoClient_SameSignOn : Migration
{
    public override void Up()
    {
        this.Rename.Table("SsoClient").To("SameSignOn");
    }

    public override void Down()
    {
        this.Rename.Table("SameSignOn").To("SsoClient");
    }
}