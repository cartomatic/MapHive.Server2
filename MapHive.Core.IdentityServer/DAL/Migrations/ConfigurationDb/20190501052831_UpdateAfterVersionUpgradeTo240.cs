using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MapHive.Core.IdentityServer.DAL.Migrations.ConfigurationDb
{
    public partial class UpdateAfterVersionUpgradeTo240 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "configuration",
                table: "IdentityResources",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "NonEditable",
                schema: "configuration",
                table: "IdentityResources",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                schema: "configuration",
                table: "IdentityResources",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                schema: "configuration",
                table: "ClientSecrets",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                schema: "configuration",
                table: "ClientSecrets",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "configuration",
                table: "ClientSecrets",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "configuration",
                table: "Clients",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DeviceCodeLifetime",
                schema: "configuration",
                table: "Clients",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAccessed",
                schema: "configuration",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NonEditable",
                schema: "configuration",
                table: "Clients",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                schema: "configuration",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCodeType",
                schema: "configuration",
                table: "Clients",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserSsoLifetime",
                schema: "configuration",
                table: "Clients",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                schema: "configuration",
                table: "ApiSecrets",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                schema: "configuration",
                table: "ApiSecrets",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "configuration",
                table: "ApiSecrets",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "configuration",
                table: "ApiResources",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAccessed",
                schema: "configuration",
                table: "ApiResources",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NonEditable",
                schema: "configuration",
                table: "ApiResources",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                schema: "configuration",
                table: "ApiResources",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApiProperties",
                schema: "configuration",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Key = table.Column<string>(maxLength: 250, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: false),
                    ApiResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiProperties_ApiResources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalSchema: "configuration",
                        principalTable: "ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityProperties",
                schema: "configuration",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Key = table.Column<string>(maxLength: 250, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: false),
                    IdentityResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityProperties_IdentityResources_IdentityResourceId",
                        column: x => x.IdentityResourceId,
                        principalSchema: "configuration",
                        principalTable: "IdentityResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiProperties_ApiResourceId",
                schema: "configuration",
                table: "ApiProperties",
                column: "ApiResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProperties_IdentityResourceId",
                schema: "configuration",
                table: "IdentityProperties",
                column: "IdentityResourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiProperties",
                schema: "configuration");

            migrationBuilder.DropTable(
                name: "IdentityProperties",
                schema: "configuration");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "configuration",
                table: "IdentityResources");

            migrationBuilder.DropColumn(
                name: "NonEditable",
                schema: "configuration",
                table: "IdentityResources");

            migrationBuilder.DropColumn(
                name: "Updated",
                schema: "configuration",
                table: "IdentityResources");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "configuration",
                table: "ClientSecrets");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "configuration",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "DeviceCodeLifetime",
                schema: "configuration",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "LastAccessed",
                schema: "configuration",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "NonEditable",
                schema: "configuration",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Updated",
                schema: "configuration",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "UserCodeType",
                schema: "configuration",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "UserSsoLifetime",
                schema: "configuration",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "configuration",
                table: "ApiSecrets");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "configuration",
                table: "ApiResources");

            migrationBuilder.DropColumn(
                name: "LastAccessed",
                schema: "configuration",
                table: "ApiResources");

            migrationBuilder.DropColumn(
                name: "NonEditable",
                schema: "configuration",
                table: "ApiResources");

            migrationBuilder.DropColumn(
                name: "Updated",
                schema: "configuration",
                table: "ApiResources");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                schema: "configuration",
                table: "ClientSecrets",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 4000);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                schema: "configuration",
                table: "ClientSecrets",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                schema: "configuration",
                table: "ApiSecrets",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 4000);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                schema: "configuration",
                table: "ApiSecrets",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 250);
        }
    }
}
