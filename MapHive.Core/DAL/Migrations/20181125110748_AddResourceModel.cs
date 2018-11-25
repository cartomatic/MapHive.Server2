using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MapHive.Core.DAL.Migrations
{
    public partial class AddResourceModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "profile_picture_id",
                schema: "mh_meta",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "profile_picture",
                schema: "mh_meta",
                table: "users",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "resources",
                schema: "mh_meta",
                columns: table => new
                {
                    uuid = table.Column<Guid>(nullable: false),
                    created_by = table.Column<Guid>(nullable: true),
                    last_modified_by = table.Column<Guid>(nullable: true),
                    create_date_utc = table.Column<DateTime>(nullable: true),
                    modify_date_utc = table.Column<DateTime>(nullable: true),
                    end_date_utc = table.Column<DateTime>(nullable: true),
                    owner_id = table.Column<Guid>(nullable: true),
                    owner_type_id = table.Column<Guid>(nullable: true),
                    identifier = table.Column<string>(nullable: true),
                    original_file_name = table.Column<string>(nullable: true),
                    mime = table.Column<string>(nullable: true),
                    data = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_resources", x => x.uuid);
                });

            migrationBuilder.CreateIndex(
                name: "idx_create_date_resource",
                schema: "mh_meta",
                table: "resources",
                column: "create_date_utc");

            migrationBuilder.CreateIndex(
                name: "idx_owner_id_resource",
                schema: "mh_meta",
                table: "resources",
                column: "owner_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "resources",
                schema: "mh_meta");

            migrationBuilder.DropColumn(
                name: "profile_picture",
                schema: "mh_meta",
                table: "users");

            migrationBuilder.AddColumn<Guid>(
                name: "profile_picture_id",
                schema: "mh_meta",
                table: "users",
                nullable: true);
        }
    }
}
