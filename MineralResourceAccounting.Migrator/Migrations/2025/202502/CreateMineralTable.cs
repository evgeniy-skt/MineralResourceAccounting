using FluentMigrator;

namespace MineralResourceAccounting.Migrator.Migrations._2025._202502;

[Migration(20250203)]
public class CreateMineralTable : Migration
{
    public override void Up()
    {
        Create.Table("Minerals")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("Name").AsString()
            .WithColumn("Type").AsString()
            .WithColumn("Lat").AsFloat()
            .WithColumn("Lon").AsFloat()
            .WithColumn("AreaName").AsString()
            .WithColumn("ValueM3").AsInt64();
    }

    public override void Down()
    {
        Delete.Table("Minerals");
    }
}