using FluentMigrator;

namespace GalliumPlus.WebApi.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_021)]
public class AddColumn_User_deleted : Migration
{
    public override void Up()
    {
        this.Alter.Table("User").AddColumn("deleted").AsBoolean().NotNullable();
    }

    public override void Down()
    {
        this.Delete.Column("deleted").FromTable("User");
    }
}