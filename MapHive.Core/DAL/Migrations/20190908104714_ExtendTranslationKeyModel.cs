using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MapHive.Core.DAL.Migrations
{
    public partial class ExtendTranslationKeyModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "inherited",
                schema: "mh_localization",
                table: "translation_keys",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "overwrites",
                schema: "mh_localization",
                table: "translation_keys",
                nullable: true);

            //migrationBuilder.CreateTable(
            //    name: "translation_keys_extended",
            //    schema: "mh_localization",
            //    columns: table => new
            //    {
            //        uuid = table.Column<Guid>(nullable: false),
            //        created_by = table.Column<Guid>(nullable: true),
            //        last_modified_by = table.Column<Guid>(nullable: true),
            //        create_date_utc = table.Column<DateTime>(nullable: true),
            //        modify_date_utc = table.Column<DateTime>(nullable: true),
            //        end_date_utc = table.Column<DateTime>(nullable: true),
            //        custom_data = table.Column<string>(nullable: true),
            //        localization_class_uuid = table.Column<Guid>(nullable: false),
            //        key = table.Column<string>(nullable: true),
            //        inherited = table.Column<bool>(nullable: true),
            //        overwrites = table.Column<bool>(nullable: true),
            //        translations = table.Column<string>(nullable: true),
            //        application_name = table.Column<string>(nullable: true),
            //        class_name = table.Column<string>(nullable: true),
            //        inherited_class_name = table.Column<string>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("pk_translation_keys_extended", x => x.uuid);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "idx_translation_key_extended_create_date",
            //    schema: "mh_localization",
            //    table: "translation_keys_extended",
            //    column: "create_date_utc");

            //migrationBuilder.CreateIndex(
            //    name: "idx_translation_key_uq_localization_class_translation_key",
            //    schema: "mh_localization",
            //    table: "translation_keys_extended",
            //    columns: new[] { "localization_class_uuid", "key" },
            //    unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "translation_keys_extended",
            //    schema: "mh_localization");

            migrationBuilder.DropColumn(
                name: "inherited",
                schema: "mh_localization",
                table: "translation_keys");

            migrationBuilder.DropColumn(
                name: "overwrites",
                schema: "mh_localization",
                table: "translation_keys");
        }
    }
}
