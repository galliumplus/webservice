using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_00_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_00_005)]
public class CreateTable_Session : Migration
{
    public override void Up()
    {
        this.Create.Table("Session")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("token").AsCustom("CHAR(20) CHARACTER SET ascii COLLATE ascii_bin").NotNullable().Unique()
            .WithColumn("lastUse").AsDateTime2().NotNullable()
            .WithColumn("expiration").AsDateTime2().NotNullable()
            .WithColumn("user").AsInt64().Nullable()
            .WithColumn("client").AsInt64().NotNullable();
    }

    public override void Down()
    {
        this.Delete.Table("Session");
    }
}