using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_01_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_01_00_006)]
public class AlterColumn_AuditLog_user : Migration
{
    public override void Up()
    {
        this.Alter.Table("AuditLog").AlterColumn("user").AsInt64().Nullable();
    }

    public override void Down()
    {
        this.Alter.Table("AuditLog").AlterColumn("user").AsInt64().NotNullable();
    }
}