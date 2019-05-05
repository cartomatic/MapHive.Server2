using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MapHive.Core.DAL.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "mh_meta");

            migrationBuilder.EnsureSchema(
                name: "mh_localization");

            migrationBuilder.CreateTable(
                name: "email_templates",
                schema: "mh_localization",
                columns: table => new
                {
                    uuid = table.Column<Guid>(nullable: false),
                    created_by = table.Column<Guid>(nullable: true),
                    last_modified_by = table.Column<Guid>(nullable: true),
                    create_date_utc = table.Column<DateTime>(nullable: true),
                    modify_date_utc = table.Column<DateTime>(nullable: true),
                    end_date_utc = table.Column<DateTime>(nullable: true),
                    custom_data = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    application_identifier = table.Column<string>(nullable: true),
                    identifier = table.Column<string>(nullable: true),
                    is_body_html = table.Column<bool>(nullable: false),
                    translations = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_email_templates", x => x.uuid);
                });

            migrationBuilder.CreateTable(
                name: "langs",
                schema: "mh_localization",
                columns: table => new
                {
                    uuid = table.Column<Guid>(nullable: false),
                    created_by = table.Column<Guid>(nullable: true),
                    last_modified_by = table.Column<Guid>(nullable: true),
                    create_date_utc = table.Column<DateTime>(nullable: true),
                    modify_date_utc = table.Column<DateTime>(nullable: true),
                    end_date_utc = table.Column<DateTime>(nullable: true),
                    custom_data = table.Column<string>(nullable: true),
                    lang_code = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    is_default = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_langs", x => x.uuid);
                });

            migrationBuilder.CreateTable(
                name: "localization_classes",
                schema: "mh_localization",
                columns: table => new
                {
                    uuid = table.Column<Guid>(nullable: false),
                    created_by = table.Column<Guid>(nullable: true),
                    last_modified_by = table.Column<Guid>(nullable: true),
                    create_date_utc = table.Column<DateTime>(nullable: true),
                    modify_date_utc = table.Column<DateTime>(nullable: true),
                    end_date_utc = table.Column<DateTime>(nullable: true),
                    custom_data = table.Column<string>(nullable: true),
                    application_name = table.Column<string>(nullable: true),
                    class_name = table.Column<string>(nullable: true),
                    inherited_class_name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_localization_classes", x => x.uuid);
                });

            migrationBuilder.CreateTable(
                name: "translation_keys",
                schema: "mh_localization",
                columns: table => new
                {
                    uuid = table.Column<Guid>(nullable: false),
                    created_by = table.Column<Guid>(nullable: true),
                    last_modified_by = table.Column<Guid>(nullable: true),
                    create_date_utc = table.Column<DateTime>(nullable: true),
                    modify_date_utc = table.Column<DateTime>(nullable: true),
                    end_date_utc = table.Column<DateTime>(nullable: true),
                    custom_data = table.Column<string>(nullable: true),
                    localization_class_uuid = table.Column<Guid>(nullable: false),
                    key = table.Column<string>(nullable: true),
                    translations = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_translation_keys", x => x.uuid);
                });

            migrationBuilder.CreateTable(
                name: "applications",
                schema: "mh_meta",
                columns: table => new
                {
                    uuid = table.Column<Guid>(nullable: false),
                    created_by = table.Column<Guid>(nullable: true),
                    last_modified_by = table.Column<Guid>(nullable: true),
                    create_date_utc = table.Column<DateTime>(nullable: true),
                    modify_date_utc = table.Column<DateTime>(nullable: true),
                    end_date_utc = table.Column<DateTime>(nullable: true),
                    custom_data = table.Column<string>(nullable: true),
                    short_name = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    urls = table.Column<string>(nullable: true),
                    use_splashscreen = table.Column<bool>(nullable: false),
                    requires_auth = table.Column<bool>(nullable: false),
                    is_common = table.Column<bool>(nullable: false),
                    is_default = table.Column<bool>(nullable: false),
                    is_home = table.Column<bool>(nullable: false),
                    is_hive = table.Column<bool>(nullable: false),
                    is_api = table.Column<bool>(nullable: false),
                    has_desktop = table.Column<bool>(nullable: false),
                    has_mobile = table.Column<bool>(nullable: false),
                    provider_id = table.Column<Guid>(nullable: true),
                    visual_identification = table.Column<string>(nullable: true),
                    license_options = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_applications", x => x.uuid);
                });

            migrationBuilder.CreateTable(
                name: "links",
                schema: "mh_meta",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    parent_uuid = table.Column<Guid>(nullable: false),
                    child_uuid = table.Column<Guid>(nullable: false),
                    parent_type_uuid = table.Column<Guid>(nullable: false),
                    child_type_uuid = table.Column<Guid>(nullable: false),
                    sort_order = table.Column<int>(nullable: true),
                    link_data = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_links", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "object_types",
                schema: "mh_meta",
                columns: table => new
                {
                    uuid = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_object_types", x => x.uuid);
                });

            migrationBuilder.CreateTable(
                name: "organization_databases",
                schema: "mh_meta",
                columns: table => new
                {
                    uuid = table.Column<Guid>(nullable: false),
                    created_by = table.Column<Guid>(nullable: true),
                    last_modified_by = table.Column<Guid>(nullable: true),
                    create_date_utc = table.Column<DateTime>(nullable: true),
                    modify_date_utc = table.Column<DateTime>(nullable: true),
                    end_date_utc = table.Column<DateTime>(nullable: true),
                    custom_data = table.Column<string>(nullable: true),
                    organization_id = table.Column<Guid>(nullable: false),
                    identifier = table.Column<string>(nullable: true),
                    provider = table.Column<int>(nullable: false),
                    server_host = table.Column<string>(nullable: true),
                    ServerName = table.Column<string>(nullable: true),
                    server_port = table.Column<int>(nullable: true),
                    db_name = table.Column<string>(nullable: true),
                    service_db_name = table.Column<string>(nullable: true),
                    user_name = table.Column<string>(nullable: true),
                    password = table.Column<string>(nullable: true),
                    service_user_name = table.Column<string>(nullable: true),
                    service_user_password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization_databases", x => x.uuid);
                });

            migrationBuilder.CreateTable(
                name: "organizations",
                schema: "mh_meta",
                columns: table => new
                {
                    uuid = table.Column<Guid>(nullable: false),
                    created_by = table.Column<Guid>(nullable: true),
                    last_modified_by = table.Column<Guid>(nullable: true),
                    create_date_utc = table.Column<DateTime>(nullable: true),
                    modify_date_utc = table.Column<DateTime>(nullable: true),
                    end_date_utc = table.Column<DateTime>(nullable: true),
                    custom_data = table.Column<string>(nullable: true),
                    slug = table.Column<string>(nullable: true),
                    display_name = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    location = table.Column<string>(nullable: true),
                    url = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    gravatar_email = table.Column<string>(nullable: true),
                    profile_picture_id = table.Column<Guid>(nullable: true),
                    billing_email = table.Column<string>(nullable: true),
                    billing_address = table.Column<string>(nullable: true),
                    billing_extra_info = table.Column<string>(nullable: true),
                    license_options = table.Column<string>(nullable: true),
                    visual_identification = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organizations", x => x.uuid);
                });

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
                    custom_data = table.Column<string>(nullable: true),
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

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "mh_meta",
                columns: table => new
                {
                    uuid = table.Column<Guid>(nullable: false),
                    created_by = table.Column<Guid>(nullable: true),
                    last_modified_by = table.Column<Guid>(nullable: true),
                    create_date_utc = table.Column<DateTime>(nullable: true),
                    modify_date_utc = table.Column<DateTime>(nullable: true),
                    end_date_utc = table.Column<DateTime>(nullable: true),
                    custom_data = table.Column<string>(nullable: true),
                    identifier = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    privileges = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.uuid);
                });

            migrationBuilder.CreateTable(
                name: "teams",
                schema: "mh_meta",
                columns: table => new
                {
                    uuid = table.Column<Guid>(nullable: false),
                    created_by = table.Column<Guid>(nullable: true),
                    last_modified_by = table.Column<Guid>(nullable: true),
                    create_date_utc = table.Column<DateTime>(nullable: true),
                    modify_date_utc = table.Column<DateTime>(nullable: true),
                    end_date_utc = table.Column<DateTime>(nullable: true),
                    custom_data = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_teams", x => x.uuid);
                });

            migrationBuilder.CreateTable(
                name: "tokens",
                schema: "mh_meta",
                columns: table => new
                {
                    uuid = table.Column<Guid>(nullable: false),
                    created_by = table.Column<Guid>(nullable: true),
                    last_modified_by = table.Column<Guid>(nullable: true),
                    create_date_utc = table.Column<DateTime>(nullable: true),
                    modify_date_utc = table.Column<DateTime>(nullable: true),
                    end_date_utc = table.Column<DateTime>(nullable: true),
                    custom_data = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    organization_id = table.Column<Guid>(nullable: true),
                    application_id = table.Column<Guid>(nullable: true),
                    referrers = table.Column<string>(nullable: true),
                    can_ignore_referrer = table.Column<bool>(nullable: false),
                    is_master = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tokens", x => x.uuid);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "mh_meta",
                columns: table => new
                {
                    uuid = table.Column<Guid>(nullable: false),
                    created_by = table.Column<Guid>(nullable: true),
                    last_modified_by = table.Column<Guid>(nullable: true),
                    create_date_utc = table.Column<DateTime>(nullable: true),
                    modify_date_utc = table.Column<DateTime>(nullable: true),
                    end_date_utc = table.Column<DateTime>(nullable: true),
                    custom_data = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    is_account_closed = table.Column<bool>(nullable: false),
                    is_account_verified = table.Column<bool>(nullable: false),
                    forename = table.Column<string>(nullable: true),
                    surname = table.Column<string>(nullable: true),
                    full_name = table.Column<string>(nullable: true),
                    contact_phone = table.Column<string>(nullable: true),
                    slug = table.Column<string>(nullable: true),
                    bio = table.Column<string>(nullable: true),
                    company = table.Column<string>(nullable: true),
                    department = table.Column<string>(nullable: true),
                    location = table.Column<string>(nullable: true),
                    gravatar_email = table.Column<string>(nullable: true),
                    profile_picture = table.Column<string>(nullable: true),
                    is_org_user = table.Column<bool>(nullable: false),
                    parent_org_id = table.Column<Guid>(nullable: true),
                    visible_in_catalogue = table.Column<bool>(nullable: false),
                    user_org_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.uuid);
                });

            migrationBuilder.CreateIndex(
                name: "idx_email_template_localization_app_identifier",
                schema: "mh_localization",
                table: "email_templates",
                column: "application_identifier");

            migrationBuilder.CreateIndex(
                name: "idx_email_template_localization_create_date",
                schema: "mh_localization",
                table: "email_templates",
                column: "create_date_utc");

            migrationBuilder.CreateIndex(
                name: "idx_email_template_localization_uq_app_name_translation_identifier",
                schema: "mh_localization",
                table: "email_templates",
                columns: new[] { "application_identifier", "identifier" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_lang_create_date",
                schema: "mh_localization",
                table: "langs",
                column: "create_date_utc");

            migrationBuilder.CreateIndex(
                name: "idx_localization_class_create_date",
                schema: "mh_localization",
                table: "localization_classes",
                column: "create_date_utc");

            migrationBuilder.CreateIndex(
                name: "idx_localization_class_uq_app_name_class_name",
                schema: "mh_localization",
                table: "localization_classes",
                columns: new[] { "application_name", "class_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_translation_key_create_date",
                schema: "mh_localization",
                table: "translation_keys",
                column: "create_date_utc");

            migrationBuilder.CreateIndex(
                name: "idx_translation_key_uq_localization_class_translation_key",
                schema: "mh_localization",
                table: "translation_keys",
                columns: new[] { "localization_class_uuid", "key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_application_create_date",
                schema: "mh_meta",
                table: "applications",
                column: "create_date_utc");

            migrationBuilder.CreateIndex(
                name: "idx_application_uq_short_name",
                schema: "mh_meta",
                table: "applications",
                column: "short_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_link_child_type_uuid",
                schema: "mh_meta",
                table: "links",
                column: "child_type_uuid");

            migrationBuilder.CreateIndex(
                name: "idx_link_child_uuid",
                schema: "mh_meta",
                table: "links",
                column: "child_uuid");

            migrationBuilder.CreateIndex(
                name: "idx_link_parent_type_uuid",
                schema: "mh_meta",
                table: "links",
                column: "parent_type_uuid");

            migrationBuilder.CreateIndex(
                name: "idx_link_parent_uuid",
                schema: "mh_meta",
                table: "links",
                column: "parent_uuid");

            migrationBuilder.CreateIndex(
                name: "idx_object_type_uq_name",
                schema: "mh_meta",
                table: "object_types",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_organization_database_create_date",
                schema: "mh_meta",
                table: "organization_databases",
                column: "create_date_utc");

            migrationBuilder.CreateIndex(
                name: "idx_organization_database_uq_org_uuid_identifier",
                schema: "mh_meta",
                table: "organization_databases",
                columns: new[] { "organization_id", "identifier" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_organization_create_date",
                schema: "mh_meta",
                table: "organizations",
                column: "create_date_utc");

            migrationBuilder.CreateIndex(
                name: "idx_organization_uq_slug",
                schema: "mh_meta",
                table: "organizations",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_resource_create_date",
                schema: "mh_meta",
                table: "resources",
                column: "create_date_utc");

            migrationBuilder.CreateIndex(
                name: "idx_resource_owner_id",
                schema: "mh_meta",
                table: "resources",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "idx_role_create_date",
                schema: "mh_meta",
                table: "roles",
                column: "create_date_utc");

            migrationBuilder.CreateIndex(
                name: "idx_team_create_date",
                schema: "mh_meta",
                table: "teams",
                column: "create_date_utc");

            migrationBuilder.CreateIndex(
                name: "idx_token_create_date",
                schema: "mh_meta",
                table: "tokens",
                column: "create_date_utc");

            migrationBuilder.CreateIndex(
                name: "idx_map_hive_user_create_date",
                schema: "mh_meta",
                table: "users",
                column: "create_date_utc");

            migrationBuilder.CreateIndex(
                name: "idx_map_hive_user_uq_email",
                schema: "mh_meta",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_map_hive_user_uq_slug",
                schema: "mh_meta",
                table: "users",
                column: "slug",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "email_templates",
                schema: "mh_localization");

            migrationBuilder.DropTable(
                name: "langs",
                schema: "mh_localization");

            migrationBuilder.DropTable(
                name: "localization_classes",
                schema: "mh_localization");

            migrationBuilder.DropTable(
                name: "translation_keys",
                schema: "mh_localization");

            migrationBuilder.DropTable(
                name: "applications",
                schema: "mh_meta");

            migrationBuilder.DropTable(
                name: "links",
                schema: "mh_meta");

            migrationBuilder.DropTable(
                name: "object_types",
                schema: "mh_meta");

            migrationBuilder.DropTable(
                name: "organization_databases",
                schema: "mh_meta");

            migrationBuilder.DropTable(
                name: "organizations",
                schema: "mh_meta");

            migrationBuilder.DropTable(
                name: "resources",
                schema: "mh_meta");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "mh_meta");

            migrationBuilder.DropTable(
                name: "teams",
                schema: "mh_meta");

            migrationBuilder.DropTable(
                name: "tokens",
                schema: "mh_meta");

            migrationBuilder.DropTable(
                name: "users",
                schema: "mh_meta");
        }
    }
}
