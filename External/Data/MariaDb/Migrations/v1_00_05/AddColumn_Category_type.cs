using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_027)]
public class AddColumn_Category_type : Migration
{
    public override void Up()
    {
        this.Alter.Table("Category").AddColumn("type").AsInt16().NotNullable().WithDefaultValue(1);
        this.Execute.Sql("ALTER TABLE `Category` ADD CONSTRAINT ENUM_Category_type CHECK (`type` BETWEEN 1 AND 3);");;
    }

    public override void Down()
    {
        this.Delete.Column("event").FromTable("Category");
    }
}