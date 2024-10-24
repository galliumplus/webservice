using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_01_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_01_00_001)]
public class RenameColumn_Client_revoked_allowed : Migration
{
    public override void Up()
    {
        this.Rename.Column("revoked").OnTable("Client").To("allowed");
    }

    public override void Down()
    {
        this.Rename.Column("allowed").OnTable("Client").To("revoked");
    }
}