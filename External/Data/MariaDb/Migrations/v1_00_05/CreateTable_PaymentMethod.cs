using FluentMigrator;

namespace GalliumPlus.Data.MariaDb.Migrations.v1_00_05;

// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
[Migration(1_00_05_024)]
public class CreateTable_PaymentMethod : Migration
{
    public override void Up()
    {
        this.Create.Table("PaymentMethod")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("type").AsInt16().NotNullable()
            .WithColumn("user").AsInt64().ForeignKey("FK_PaymentMethod_User_user", "User", "id").Nullable()
            .WithColumn("data").AsCustom("JSON")
            .WithColumn("tag1").AsString(20).Nullable();
        this.Execute.Sql("ALTER TABLE `PaymentMethod` ADD CONSTRAINT ENUM_PaymentMethod_type CHECK (`type` BETWEEN 1 AND 4);");
    }

    public override void Down()
    {
        this.Delete.Table("PaymentMethod");
    }
}