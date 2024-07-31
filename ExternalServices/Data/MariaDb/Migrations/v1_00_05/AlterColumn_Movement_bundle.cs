using FluentMigrator;

namespace GalliumPlus.WebApi.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_029)]
public class AlterColumn_Movement_bundle : Migration
{
    public override void Up()
    {
        this.Alter.Table("Movement").AlterColumn("bundle").AsInt64().Nullable();
    }

    public override void Down()
    {
        this.Alter.Table("Movement").AlterColumn("bundle").AsInt64().NotNullable();
    }
}