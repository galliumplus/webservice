using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_01_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_01_00_004)]
public class AddColumn_SameSignOn_displayName : Migration
{
    public override void Up()
    {
        this.Alter.Table("SameSignOn").AddColumn("displayName").AsString(50).Nullable();
    }

    public override void Down()
    {
        this.Delete.Column("displayName").FromTable("SameSignOn");
    }
}