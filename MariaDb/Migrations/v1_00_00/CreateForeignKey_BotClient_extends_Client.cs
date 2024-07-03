using System.Data;
using FluentMigrator;

namespace GalliumPlus.WebApi.Data.MariaDb.Migrations.v1_00_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(2024_07_03_016)]
public class CreateForeignKey_BotClient_extends_Client : Migration
{
	public override void Up()
	{ 
		this.Create.ForeignKey("FK_BotClient_extends_Client")
			.FromTable("BotClient").ForeignColumn("id").ToTable("Client").PrimaryColumn("id")
			.OnUpdate(Rule.Cascade) .OnDelete(Rule.Cascade);
	}

	public override void Down()
	{
		this.Delete.ForeignKey("BotClient_extends_Client");
	}
}
