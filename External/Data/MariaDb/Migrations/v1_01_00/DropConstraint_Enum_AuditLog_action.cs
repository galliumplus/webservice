using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_01_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_01_00_005)]
public class DropConstraint_Enum_AuditLog_action : Migration
{
    public override void Up()
    {
        this.Execute.Sql("ALTER TABLE `AuditLog` DROP CONSTRAINT ENUM_AuditLog_action;");
    }

    public override void Down()
    {
        this.Execute.Sql("ALTER TABLE `AuditLog` ADD CONSTRAINT ENUM_AuditLog_action CHECK (`action` BETWEEN 1 AND 1);");
    }
}