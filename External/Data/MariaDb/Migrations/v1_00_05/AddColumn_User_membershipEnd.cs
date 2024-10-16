using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_028)]
public class AddColumn_User_membershipEnd : Migration
{
    public override void Up()
    {
        this.Alter.Table("User").AddColumn("membershipEnd").AsDateTime2().Nullable();
    }

    public override void Down()
    {
        this.Delete.Column("membershipEnd").FromTable("User");
    }
}