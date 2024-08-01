using FluentMigrator;

namespace GalliumPlus.WebApi.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_020)]
public class DropColumn_SameSignOn_usesApi : Migration
{
    public override void Up()
    {
        this.Delete.Column("usesApi").FromTable("SameSignOn");
    }

    public override void Down()
    {
        this.Create.Column("usesApi").OnTable("SameSignOn").AsBoolean().NotNullable().WithDefaultValue(false);
    }
}