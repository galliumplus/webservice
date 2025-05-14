using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_04_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_04_00_001)]
public class AlterColumn_Client_allowed : Migration
{
    public override void Up()
    {
        this.Alter.Table("Client").AlterColumn("allowed").AsInt32().NotNullable();
    }

    public override void Down()
    {
        this.Alter.Table("Client").AlterColumn("allowed").AsInt32().Nullable();
    }
}