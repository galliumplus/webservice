using System.Data;
using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_00_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_00_019)]
public class CreateForeignKey_HistoryAction_HistoryUser_actor : Migration
{
	public override void Up()
	{ 
		this.Create.ForeignKey("FK_HistoryAction_HistoryUser_actor")
			.FromTable("HistoryAction").ForeignColumn("actor").ToTable("HistoryUser").PrimaryColumn("id")
			.OnUpdate(Rule.None).OnDelete(Rule.None);
	}

	public override void Down()
	{
		this.Delete.ForeignKey("HistoryAction_HistoryUser_actor");
	}
}
