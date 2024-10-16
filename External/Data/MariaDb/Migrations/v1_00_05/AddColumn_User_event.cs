using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_022)]
public class AddColumn_User_event : Migration
{
    public override void Up()
    {
        this.Alter.Table("User").AddColumn("event").AsInt64().WithDefaultValue(1).ForeignKey("FK_User_Event_event", "Event", "id");
    }

    public override void Down()
    {
        this.Delete.ForeignKey("FK_User_Event_event").OnTable("User");
        this.Delete.Column("event").FromTable("User");
    }
}