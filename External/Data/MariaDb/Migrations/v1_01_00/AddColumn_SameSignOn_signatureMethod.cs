using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_01_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_01_00_002)]
public class AddColumn_SameSignOn_signatureMethod : Migration
{
    public override void Up()
    {
        this.Alter.Table("SameSignOn").AddColumn("signatureMethod").AsInt16().NotNullable().WithDefaultValue(1);
        this.Execute.Sql("ALTER TABLE `SameSignOn` ADD CONSTRAINT ENUM_SameSignOn_signatureMethod CHECK (`signatureMethod` = 1);");
    }

    public override void Down()
    {
        this.Delete.Column("signatureMethod").FromTable("SameSignOn");
    }
}