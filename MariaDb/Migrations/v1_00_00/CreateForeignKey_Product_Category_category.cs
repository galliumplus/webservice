using System.Data;
using FluentMigrator;

namespace GalliumPlus.WebApi.Data.MariaDb.Migrations.v1_00_00;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(2024_07_03_018)]
public class CreateForeignKey_Product_Category_category : Migration
{
	public override void Up()
	{ 
		this.Create.ForeignKey("FK_Product_Category_category")
			.FromTable("Product").ForeignColumn("category").ToTable("Category").PrimaryColumn("id")
			.OnUpdate(Rule.Cascade) .OnDelete(Rule.None);
	}

	public override void Down()
	{
		this.Delete.ForeignKey("Product_Category_category");
	}
}
