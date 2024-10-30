using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_01_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_01_00_003)]
public class AddColumn_SameSignOn_scope : Migration
{
    public override void Up()
    {
        this.Alter.Table("SameSignOn").AddColumn("scope").AsInt16().NotNullable().WithDefaultValue(0);
    }

    public override void Down()
    {
        this.Delete.Column("scope").FromTable("SameSignOn");
    }
}