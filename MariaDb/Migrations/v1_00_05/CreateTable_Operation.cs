using FluentMigrator;

namespace GalliumPlus.WebApi.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_013)]
public class CreateTable_Operation : Migration
{
    public override void Up()
    {
        this.Create.Table("Operation")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("code").AsInt16().NotNullable()
            .WithColumn("thirdParty").AsString(20).NotNullable()
            .WithColumn("total").AsDecimal(6, 2).NotNullable()
            .WithColumn("description").AsString(50).NotNullable()
            .WithColumn("date").AsDateTime2().NotNullable();
        this.Execute.Sql("ALTER TABLE `Operation` ADD CONSTRAINT ENUM_Operation_code CHECK (`code` BETWEEN 1 AND 4);");
    }

    public override void Down()
    {
        this.Delete.Table("Operation");
    }
}