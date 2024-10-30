using System.Data;
using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_00_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_00_012)]
public class CreateForeignKey_User_Role_role : Migration
{
    public override void Up()
    {
        this.Create.ForeignKey("FK_User_Role_role")
            .FromTable("User").ForeignColumn("role").ToTable("Role").PrimaryColumn("id")
            .OnUpdate(Rule.Cascade).OnDelete(Rule.None);
    }

    public override void Down()
    {
        this.Delete.ForeignKey("FK_User_Role_role");
    }
}