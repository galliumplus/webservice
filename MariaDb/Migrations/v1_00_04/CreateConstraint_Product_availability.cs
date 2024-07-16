using FluentMigrator;

namespace GalliumPlus.WebApi.Data.MariaDb.Migrations.v1_00_04;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(2024_07_03_021)]
public class CreateConstraint_Product_availability : Migration
{
    public override void Up()
    {
        this.Execute.Sql("ALTER TABLE `Product` ADD CONSTRAINT ENUM_Product_availability CHECK (`availability` BETWEEN 0 AND 2);");
    }

    public override void Down()
    {
        this.Execute.Sql("ALTER TABLE `Product` DROP CONSTRAINT ENUM_Product_availability;");
    }
}