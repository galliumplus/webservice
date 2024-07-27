using FluentMigrator;

namespace GalliumPlus.WebApi.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_002)]
public class CreateTable_EventCourse : Migration
{
    public override void Up()
    {
        this.Create.Table("EventCourse")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("event").AsInt64().ForeignKey("FK_EventCourse_Event_event", "Event", "id").NotNullable()
            .WithColumn("beginning").AsDateTime2().NotNullable()
            .WithColumn("end").AsDateTime2().Nullable()
            .WithColumn("mandatoryParticipation").AsBoolean().NotNullable();
    }

    public override void Down()
    {
        this.Delete.Table("EventCourse");
    }
}