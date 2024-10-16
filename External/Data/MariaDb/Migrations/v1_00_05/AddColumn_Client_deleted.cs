using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_017)]
public class AddColumn_Client_deleted : Migration
{
    public override void Up()
    {
        this.Alter.Table("Client").AddColumn("deleted").AsBoolean().NotNullable();
    }

    public override void Down()
    {
        this.Delete.Column("deleted").FromTable("Client");
    }
}