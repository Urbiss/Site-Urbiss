using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Urbiss.Repository.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "cities",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    geometry = table.Column<MultiPolygon>(type: "geometry(multipolygon)", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", nullable: false),
                    ibgecode = table.Column<string>(type: "character varying(7)", nullable: false),
                    srid = table.Column<int>(type: "integer", nullable: false),
                    latitudecenter = table.Column<double>(type: "double precision", nullable: false),
                    longitudecenter = table.Column<double>(type: "double precision", nullable: false),
                    zoom = table.Column<int>(type: "integer", nullable: false),
                    usercreation = table.Column<string>(type: "text", nullable: false),
                    creation = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    usermodification = table.Column<string>(type: "text", nullable: true),
                    modification = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedname = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrencystamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fullname = table.Column<string>(type: "character varying(150)", nullable: false),
                    isactive = table.Column<bool>(type: "boolean", nullable: false),
                    username = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedusername = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedemail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    emailconfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    passwordhash = table.Column<string>(type: "text", nullable: true),
                    securitystamp = table.Column<string>(type: "text", nullable: true),
                    concurrencystamp = table.Column<string>(type: "text", nullable: true),
                    phonenumber = table.Column<string>(type: "text", nullable: true),
                    phonenumberconfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    twofactorenabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockoutend = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockoutenabled = table.Column<bool>(type: "boolean", nullable: false),
                    accessfailedcount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roleclaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    roleid = table.Column<int>(type: "integer", nullable: false),
                    claimtype = table.Column<string>(type: "text", nullable: true),
                    claimvalue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roleclaims", x => x.id);
                    table.ForeignKey(
                        name: "fk_roleclaims_roles_roleid",
                        column: x => x.roleid,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "surveys",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    geometry = table.Column<Polygon>(type: "geometry(polygon)", nullable: false),
                    userid = table.Column<int>(type: "integer", nullable: false),
                    surveydate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    folder = table.Column<string>(type: "character varying(40)", nullable: false),
                    srid = table.Column<int>(type: "integer", nullable: false),
                    usercreation = table.Column<string>(type: "text", nullable: false),
                    creation = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    usermodification = table.Column<string>(type: "text", nullable: true),
                    modification = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_surveys", x => x.id);
                    table.ForeignKey(
                        name: "fk_surveys_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "userclaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<int>(type: "integer", nullable: false),
                    claimtype = table.Column<string>(type: "text", nullable: true),
                    claimvalue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_userclaims", x => x.id);
                    table.ForeignKey(
                        name: "fk_userclaims_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "userlogins",
                columns: table => new
                {
                    loginprovider = table.Column<string>(type: "text", nullable: false),
                    providerkey = table.Column<string>(type: "text", nullable: false),
                    providerdisplayname = table.Column<string>(type: "text", nullable: true),
                    userid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_userlogins", x => new { x.loginprovider, x.providerkey });
                    table.ForeignKey(
                        name: "fk_userlogins_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "userroles",
                columns: table => new
                {
                    userid = table.Column<int>(type: "integer", nullable: false),
                    roleid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_userroles", x => new { x.userid, x.roleid });
                    table.ForeignKey(
                        name: "fk_userroles_roles_roleid",
                        column: x => x.roleid,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_userroles_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usersurveys",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    geometry = table.Column<Polygon>(type: "geometry(polygon)", nullable: false),
                    userid = table.Column<int>(type: "integer", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ip = table.Column<string>(type: "character varying(20)", nullable: false),
                    area = table.Column<double>(type: "double precision", nullable: false),
                    usercreation = table.Column<string>(type: "text", nullable: false),
                    creation = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    usermodification = table.Column<string>(type: "text", nullable: true),
                    modification = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usersurveys", x => x.id);
                    table.ForeignKey(
                        name: "fk_usersurveys_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "usertokens",
                columns: table => new
                {
                    userid = table.Column<int>(type: "integer", nullable: false),
                    loginprovider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usertokens", x => new { x.userid, x.loginprovider, x.name });
                    table.ForeignKey(
                        name: "fk_usertokens_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    orderid = table.Column<string>(type: "character varying(40)", nullable: false),
                    userid = table.Column<int>(type: "integer", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    usersurveyid = table.Column<long>(type: "bigint", nullable: false),
                    surveyid = table.Column<long>(type: "bigint", nullable: false),
                    price = table.Column<double>(type: "double precision", nullable: false),
                    status = table.Column<string>(type: "character varying(15)", nullable: false),
                    usercreation = table.Column<string>(type: "text", nullable: false),
                    creation = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    usermodification = table.Column<string>(type: "text", nullable: true),
                    modification = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_orders", x => x.id);
                    table.ForeignKey(
                        name: "fk_orders_surveys_surveyid",
                        column: x => x.surveyid,
                        principalTable: "surveys",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_orders_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_orders_usersurveys_usersurveyid",
                        column: x => x.usersurveyid,
                        principalTable: "usersurveys",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cities_ibgecode",
                table: "cities",
                column: "ibgecode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_orders_orderid",
                table: "orders",
                column: "orderid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_orders_surveyid",
                table: "orders",
                column: "surveyid");

            migrationBuilder.CreateIndex(
                name: "ix_orders_userid",
                table: "orders",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_orders_usersurveyid",
                table: "orders",
                column: "usersurveyid");

            migrationBuilder.CreateIndex(
                name: "ix_roleclaims_roleid",
                table: "roleclaims",
                column: "roleid");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "roles",
                column: "normalizedname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_surveys_folder",
                table: "surveys",
                column: "folder",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_surveys_userid",
                table: "surveys",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_userclaims_userid",
                table: "userclaims",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_userlogins_userid",
                table: "userlogins",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_userroles_roleid",
                table: "userroles",
                column: "roleid");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "users",
                column: "normalizedemail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "users",
                column: "normalizedusername",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_usersurveys_userid",
                table: "usersurveys",
                column: "userid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cities");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "roleclaims");

            migrationBuilder.DropTable(
                name: "userclaims");

            migrationBuilder.DropTable(
                name: "userlogins");

            migrationBuilder.DropTable(
                name: "userroles");

            migrationBuilder.DropTable(
                name: "usertokens");

            migrationBuilder.DropTable(
                name: "surveys");

            migrationBuilder.DropTable(
                name: "usersurveys");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
