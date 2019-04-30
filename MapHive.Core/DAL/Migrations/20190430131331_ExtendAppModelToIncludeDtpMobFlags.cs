using Microsoft.EntityFrameworkCore.Migrations;

namespace MapHive.Core.DAL.Migrations
{
    public partial class ExtendAppModelToIncludeDtpMobFlags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "has_desktop",
                schema: "mh_meta",
                table: "applications",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "has_mobile",
                schema: "mh_meta",
                table: "applications",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "has_desktop",
                schema: "mh_meta",
                table: "applications");

            migrationBuilder.DropColumn(
                name: "has_mobile",
                schema: "mh_meta",
                table: "applications");
        }
    }
}
