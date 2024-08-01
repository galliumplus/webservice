using FluentMigrator;

namespace GalliumPlus.WebApi.Data.MariaDb.Migrations.v1_00_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_00_011)]
public class CreateTable_HistoryUser : Migration
{
    public override void Up()
    {
        this.Create.Table("HistoryUser")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("userId").AsCustom("VARCHAR(21) CHARACTER SET ascii COLLATE ascii_bin").NotNullable().Unique();
    }

    public override void Down()
    {
        this.Delete.Table("HistoryUser");
    }
}