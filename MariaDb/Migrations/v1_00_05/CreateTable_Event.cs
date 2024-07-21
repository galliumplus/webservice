using FluentMigrator;

namespace GalliumPlus.WebApi.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_001)]
public class CreateTable_Event : Migration
{
    public override void Up()
    {
        this.Create.Table("Event")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("name").AsString(50).NotNullable()
            .WithColumn("isRecurring").AsBoolean().NotNullable()
            .WithColumn("extends").AsInt64().ForeignKey("FK_Event_Event_extends", "Event", "id").Nullable();
    }

    public override void Down()
    {
        this.Delete.Table("Event");
    }
}