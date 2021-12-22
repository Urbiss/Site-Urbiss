using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Urbiss.Repository.Migrations
{
    public partial class Voucher : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "voucherid",
                table: "orders",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "vouchers",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(15)", nullable: false),
                    area = table.Column<double>(type: "double precision", nullable: false),
                    expiration = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "character varying(10)", nullable: false),
                    usercreation = table.Column<string>(type: "text", nullable: false),
                    creation = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    usermodification = table.Column<string>(type: "text", nullable: true),
                    modification = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vouchers", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_orders_voucherid",
                table: "orders",
                column: "voucherid");

            migrationBuilder.CreateIndex(
                name: "ix_vouchers_code",
                table: "vouchers",
                column: "code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_orders_vouchers_voucherid",
                table: "orders",
                column: "voucherid",
                principalTable: "vouchers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_orders_vouchers_voucherid",
                table: "orders");

            migrationBuilder.DropTable(
                name: "vouchers");

            migrationBuilder.DropIndex(
                name: "ix_orders_voucherid",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "voucherid",
                table: "orders");
        }
    }
}
