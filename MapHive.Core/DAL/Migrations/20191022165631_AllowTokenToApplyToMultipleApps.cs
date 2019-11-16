using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MapHive.Core.DAL.Migrations
{
    public partial class AllowTokenToApplyToMultipleApps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "application_id",
                schema: "mh_meta",
                table: "tokens");

            migrationBuilder.AddColumn<string>(
                name: "application_ids",
                schema: "mh_meta",
                table: "tokens",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                schema: "mh_meta",
                table: "links",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "application_ids",
                schema: "mh_meta",
                table: "tokens");

            migrationBuilder.AddColumn<Guid>(
                name: "application_id",
                schema: "mh_meta",
                table: "tokens",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                schema: "mh_meta",
                table: "links",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
