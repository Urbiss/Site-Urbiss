using Microsoft.EntityFrameworkCore.Migrations;

namespace Urbiss.Repository.Migrations
{
    public partial class AerialPhotosSpatial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"create index aerialphotos_geom_idx on aerialphotos using gist(geometry)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop index aerialphotos_geom_idx");
        }
    }
}
