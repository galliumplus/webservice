using FluentMigrator;

namespace GalliumPlus.WebApi.Data.MariaDb.Migrations.v1_00_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_00_002)]
public class CreateTable_User : Migration
{
    public override void Up()
    {
        this.Create.Table("User")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("userId").AsCustom("VARCHAR(20) CHARACTER SET ascii COLLATE ascii_bin").NotNullable().Unique().NotNullable()
            .WithColumn("firstName").AsString(50).NotNullable()
            .WithColumn("lastName").AsString(50).NotNullable()
            .WithColumn("email").AsString(80).NotNullable()
            .WithColumn("role").AsInt64().NotNullable()
            .WithColumn("year").AsString(10).NotNullable()
            .WithColumn("deposit").AsDecimal(6,2).Nullable()
            .WithColumn("isMember").AsBoolean().NotNullable()
            .WithColumn("password").AsCustom("BINARY(32)").NotNullable().WithDefaultValue(new string('\0', 32))
            .WithColumn("salt").AsCustom("CHAR(32) CHARACTER SET ascii COLLATE ascii_bin").NotNullable().WithDefaultValue("")
            .WithColumn("registration").AsDateTime2().NotNullable();
    }

    public override void Down()
    {
        this.Delete.Table("Client");
    }
}