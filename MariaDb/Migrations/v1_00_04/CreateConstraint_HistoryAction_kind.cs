using FluentMigrator;

namespace GalliumPlus.WebApi.Data.MariaDb.Migrations.v1_00_04;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(2024_07_03_022)]
public class CreateConstraint_HistoryAction_kind : Migration
{
    public override void Up()
    {
        this.Execute.Sql("ALTER TABLE `HistoryAction` ADD CONSTRAINT ENUM_HistoryAction_kind CHECK (`kind` BETWEEN 1 AND 4);");
    }

    public override void Down()
    {
        this.Execute.Sql("ALTER TABLE `HistoryAction` DROP CONSTRAINT ENUM_HistoryAction_kind;");
    }
}