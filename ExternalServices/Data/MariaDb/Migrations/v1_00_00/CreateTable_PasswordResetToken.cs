using FluentMigrator;

namespace GalliumPlus.WebApi.Data.MariaDb.Migrations.v1_00_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_00_003)]
public class CreateTable_PasswordResetToken : Migration
{
    public override void Up()
    {
        this.Create.Table("PasswordResetToken")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("token").AsCustom("CHAR(20) CHARACTER SET ascii COLLATE ascii_bin").NotNullable().Unique()
            .WithColumn("secret").AsCustom("BINARY(32)").NotNullable().WithDefaultValue(new string('\0', 32))
            .WithColumn("salt").AsCustom("CHAR(32) CHARACTER SET ascii COLLATE ascii_bin").NotNullable().WithDefaultValue("")
            .WithColumn("expiration").AsDateTime2().NotNullable()
            .WithColumn("userId").AsCustom("VARCHAR(20) CHARACTER SET ascii COLLATE ascii_bin").NotNullable();
    }

    public override void Down()
    {
        this.Delete.Table("PasswordResetToken");
    }
}