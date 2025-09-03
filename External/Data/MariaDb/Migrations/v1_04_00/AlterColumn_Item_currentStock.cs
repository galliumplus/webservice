using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_04_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_04_00_003)]
public class AlterColumn_Item_currentStock : Migration
{
    public override void Up()
    {
        this.Alter.Table("Item").AlterColumn("currentStock").AsInt32().Nullable();
    }

    public override void Down()
    {
        this.Alter.Table("Item").AlterColumn("currentStock").AsInt32().NotNullable();
    }
}