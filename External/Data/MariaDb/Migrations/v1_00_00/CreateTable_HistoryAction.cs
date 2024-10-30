using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_00_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_00_010)]
public class CreateTable_HistoryAction : Migration
{
    public override void Up()
    {
        this.Create.Table("HistoryAction")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("text").AsString(120).NotNullable()
            .WithColumn("time").AsDateTime2().NotNullable()
            .WithColumn("kind").AsInt16().NotNullable()
            .WithColumn("actor").AsInt64().Nullable()
            .WithColumn("target").AsInt64().Nullable()
            .WithColumn("numericValue").AsDecimal(6, 2).Nullable();
    }

    public override void Down()
    {
        this.Delete.Table("HistoryAction");
    }
}