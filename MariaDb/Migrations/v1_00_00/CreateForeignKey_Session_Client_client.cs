using System.Data;
using FluentMigrator;

namespace GalliumPlus.WebApi.Data.MariaDb.Migrations.v1_00_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(2024_07_03_015)]
public class CreateForeignKey_Session_Client_client : Migration
{
	public override void Up()
	{ 
		this.Create.ForeignKey("FK_Session_Client_client")
			.FromTable("Session").ForeignColumn("client").ToTable("Client").PrimaryColumn("id")
			.OnUpdate(Rule.Cascade) .OnDelete(Rule.Cascade);
	}

	public override void Down()
	{
		this.Delete.ForeignKey("Session_Client_client");
	}
}
