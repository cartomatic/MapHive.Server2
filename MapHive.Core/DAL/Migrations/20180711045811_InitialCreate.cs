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
                    name = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    application_name = table.Column<string>(nullable: true),
                    identifier = table.Column<string>(nullable: true),
                    is_body_html = table.Column<bool>(nullable: false),
                    translations = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_email_templates", x => x.uuid);
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
                    lang_code = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    is_default = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_langs", x => x.uuid);
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
                    application_name = table.Column<string>(nullable: true),
                    class_name = table.Column<string>(nullable: true),
                    inherited_class_name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_localization_classes", x => x.uuid);
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
                    localization_class_uuid = table.Column<Guid>(nullable: false),
                    key = table.Column<string>(nullable: true),
                    translations = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_translation_keys", x => x.uuid);
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
                    provider_id = table.Column<Guid>(nullable: true),
                    license_options = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applications", x => x.uuid);
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
                    table.PrimaryKey("PK_organization_databases", x => x.uuid);
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
                    license_options = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organizations", x => x.uuid);
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
                    identifier = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    privileges = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.uuid);
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
                    name = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams", x => x.uuid);
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
                    table.PrimaryKey("PK_tokens", x => x.uuid);
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
                    is_account_closed = table.Column<bool>(nullable: false),
                    is_account_verified = table.Column<bool>(nullable: false),
                    email = table.Column<string>(nullable: true),
                    forename = table.Column<string>(nullable: true),
                    surname = table.Column<string>(nullable: true),
                    contact_phone = table.Column<string>(nullable: true),
                    slug = table.Column<string>(nullable: true),
                    bio = table.Column<string>(nullable: true),
                    company = table.Column<string>(nullable: true),
                    department = table.Column<string>(nullable: true),
                    location = table.Column<string>(nullable: true),
                    gravatar_email = table.Column<string>(nullable: true),
                    profile_picture_id = table.Column<Guid>(nullable: true),
                    is_org_user = table.Column<bool>(nullable: false),
                    parent_org_id = table.Column<Guid>(nullable: true),
                    visible_in_catalogue = table.Column<bool>(nullable: false),
                    user_org_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.uuid);
                });

            migrationBuilder.CreateIndex(
                name: "idx_app_name_emailtemplatelocalization",
                schema: "mh_localization",
                table: "email_templates",
                column: "application_name");

            migrationBuilder.CreateIndex(
                name: "idx_create_date_emailtemplatelocalization",
                schema: "mh_localization",
                table: "email_templates",
                column: "create_date_utc");

            migrationBuilder.CreateIndex(
                name: "uq_app_name_translation_identifier_emailtemplatelocalization",
                schema: "mh_localization",
                table: "email_templates",
                columns: new[] { "application_name", "identifier" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_create_date_lang",
                schema: "mh_localization",
                table: "langs",
                column: "create_date_utc");

            migrationBuilder.CreateIndex(
                name: "idx_create_date_localizationclass",
                schema: "mh_localization",
                table: "localization_classes",
                column: "create_date_utc");

            migrationBuilder.CreateIndex(
                name: "uq_app_name_class_name_localizationclass",
                schema: "mh_localization",
                table: "localization_classes",
                columns: new[] { "application_name", "class_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_create_date_translationkey",
                schema: "mh_localization",
                table: "translation_keys",
                column: "create_date_utc");

            migrationBuilder.CreateIndex(
                name: "uq_localization_class_translation_key_translationkey",
                schema: "mh_localization",
                table: "translation_keys",
                columns: new[] { "localization_class_uuid", "key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_create_date_application",
                schema: "mh_meta",
                table: "applications",
                column: "create_date_utc");

            migrationBuilder.CreateIndex(
                name: "uq_short_name_application",
                schema: "mh_meta",
                table: "applications",
                column: "short_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_child_type_uuid_link",
                schema: "mh_meta",
                table: "links",
                column: "child_type_uuid");

            migrationBuilder.CreateIndex(
                name: "idx_child_uuid_link",
                schema: "mh_meta",
                table: "links",
                column: "child_uuid");

            migrationBuilder.CreateIndex(
                name: "idx_parent_type_uuid_link",
                schema: "mh_meta",
                table: "links",
                column: "parent_type_uuid");

            migrationBuilder.CreateIndex(
                name: "idx_parent_uuid_link",
                schema: "mh_meta",
                table: "links",
                column: "parent_uuid");

            migrationBuilder.CreateIndex(
                name: "uq_name_objecttype",
                schema: "mh_meta",
                table: "object_types",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_create_date_organizationdatabase",
                schema: "mh_meta",
                table: "organization_databases",
                column: "create_date_utc");

            migrationBuilder.CreateIndex(
                name: "uq_org_uuid_identifier_organizationdatabase",
                schema: "mh_meta",
                table: "organization_databases",
                columns: new[] { "organization_id", "identifier" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_create_date_organization",
                schema: "mh_meta",
                table: "organizations",
                column: "create_date_utc");

            migrationBuilder.CreateIndex(
                name: "uq_slug_organization",
                schema: "mh_meta",
                table: "organizations",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_create_date_role",
                schema: "mh_meta",
                table: "roles",
                column: "create_date_utc");

            migrationBuilder.CreateIndex(
                name: "idx_create_date_team",
                schema: "mh_meta",
                table: "teams",
                column: "create_date_utc");

            migrationBuilder.CreateIndex(
                name: "idx_create_date_token",
                schema: "mh_meta",
                table: "tokens",
                column: "create_date_utc");

            migrationBuilder.CreateIndex(
                name: "idx_create_date_maphiveuser",
                schema: "mh_meta",
                table: "users",
                column: "create_date_utc");

            migrationBuilder.CreateIndex(
                name: "uq_email_maphiveuser",
                schema: "mh_meta",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_slug_maphiveuser",
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
