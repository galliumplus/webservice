using System.Data;
using FluentMigrator;

namespace GalliumPlus.WebApi.Data.MariaDb.Migrations.v1_00_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_00_014)]
public class CreateForeignKey_Session_User_user : Migration
{
	public override void Up()
	{ 
		this.Create.ForeignKey("FK_Session_User_user")
			.FromTable("Session").ForeignColumn("user").ToTable("User").PrimaryColumn("id")
			.OnUpdate(Rule.Cascade) .OnDelete(Rule.Cascade);
	}

	public override void Down()
	{
		this.Delete.ForeignKey("Session_User_user");
	}
}