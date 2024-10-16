using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_003)]
public class CreateRootEvent : Migration
{
    public override void Up()
    {
        this.Execute.Sql("INSERT INTO Event VALUES (1, 'Aucun', FALSE, NULL)");
    }

    public override void Down()
    {
        this.Execute.Sql("DELETE FROM Event WHERE id = 1");
    }
}