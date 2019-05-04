using Microsoft.EntityFrameworkCore.Migrations;

namespace MapHive.Core.DAL.Migrations
{
    public partial class AddModelsForOrgVisualIdentification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "visual_identification",
                schema: "mh_meta",
                table: "organizations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "visual_identification",
                schema: "mh_meta",
                table: "organizations");
        }
    }
}
