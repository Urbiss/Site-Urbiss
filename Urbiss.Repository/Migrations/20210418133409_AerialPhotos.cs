using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Urbiss.Repository.Migrations
{
    public partial class AerialPhotos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "aerialphotos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    geometry = table.Column<Polygon>(type: "geometry(point)", nullable: false),
                    name = table.Column<string>(type: "character varying(40)", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    surveyid = table.Column<long>(type: "bigint", nullable: false),
                    usercreation = table.Column<string>(type: "text", nullable: false),
                    creation = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    usermodification = table.Column<string>(type: "text", nullable: true),
                    modification = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aerialphotos", x => x.id);
                    table.ForeignKey(
                        name: "fk_aerialphotos_surveys_surveyid",
                        column: x => x.surveyid,
                        principalTable: "surveys",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_aerialphotos_surveyid",
                table: "aerialphotos",
                column: "surveyid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "aerialphotos");
        }
    }
}
