using System.Data;
using FluentMigrator;

namespace GalliumPlus.WebApi.Data.MariaDb.Migrations.v1_00_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_00_013)]
public class CreateForeignKey_PasswordResetToken_User_userId : Migration
{
    public override void Up()
    {
        this.Create.ForeignKey("FK_PasswordResetToken_User_userId")
            .FromTable("PasswordResetToken").ForeignColumn("userId").ToTable("User").PrimaryColumn("userId")
            .OnUpdate(Rule.Cascade).OnDelete(Rule.Cascade);

    }

    public override void Down()
    {
        this.Delete.ForeignKey("FK_PasswordResetToken_User_userId");
    }
}