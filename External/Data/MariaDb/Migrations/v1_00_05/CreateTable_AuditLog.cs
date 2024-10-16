using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_023)]
public class CreateTable_AuditLog : Migration
{
    public override void Up()
    {
        this.Create.Table("AuditLog")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("action").AsInt16().NotNullable()
            .WithColumn("time").AsDateTime2().NotNullable()
            .WithColumn("client").AsInt64().ForeignKey("FK_AuditLog_Client_client", "Client", "id")
            .WithColumn("user").AsInt64().ForeignKey("FK_AuditLog_User_user", "User", "id")
            .WithColumn("details").AsCustom("JSON");
        this.Execute.Sql("ALTER TABLE `AuditLog` ADD CONSTRAINT ENUM_AuditLog_action CHECK (`action` BETWEEN 1 AND 1);");
    }

    public override void Down()
    {
        this.Delete.Table("AuditLog");
    }
}