using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_015)]
public class CreateTable_Overhead : Migration
{
    public override void Up()
    {
        this.Create.Table("Overhead")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("quantity").AsInt32().NotNullable()
            .WithColumn("description").AsString(50).NotNullable()
            .WithColumn("category").AsInt64().ForeignKey("FK_Overhead_Category_category", "Category", "id").Nullable()
            .WithColumn("total").AsDecimal(6, 2).NotNullable()
            .WithColumn("operation").AsInt64().ForeignKey("FK_Overhead_Operation_operation", "Operation", "id");
    }

    public override void Down()
    {
        this.Delete.Table("Overhead");
    }
}