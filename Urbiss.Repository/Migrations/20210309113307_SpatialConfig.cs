using Microsoft.EntityFrameworkCore.Migrations;

namespace Urbiss.Repository.Migrations
{
    public partial class SpatialConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"create index cities_geom_idx on cities using gist(geometry)");
            migrationBuilder.Sql(@"create index surveys_geom_idx on surveys using gist(geometry)");
            migrationBuilder.Sql(@"create index usersurveys_geom_idx on usersurveys using gist(geometry)");

            migrationBuilder.Sql(@"select UpdateGeometrySRID('cities','geometry',4326)");
            migrationBuilder.Sql(@"select UpdateGeometrySRID('surveys','geometry',4326)");
            migrationBuilder.Sql(@"select UpdateGeometrySRID('usersurveys','geometry',4326)");

            migrationBuilder.Sql(@"insert into cities(geometry, name, ibgecode, srid, latitudecenter, longitudecenter, zoom, usercreation, creation)
                                         select st_multi(geom), upper(nome), geocodigo, 31983, -19.961507, -44.184623, 14, 'tuffi', CURRENT_TIMESTAMP from mg4326 
                                         where upper(nome) = 'BETIM'");
            migrationBuilder.Sql(@"insert into cities(geometry, name, ibgecode, srid, latitudecenter, longitudecenter, zoom, usercreation, creation)
                                         select st_multi(geom), upper(nome), geocodigo, 31983, -19.860217, -44.5951837, 14, 'tuffi', CURRENT_TIMESTAMP from mg4326 
                                         where upper(nome) = 'PARÁ DE MINAS'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop index cities_geom_idx");
            migrationBuilder.Sql(@"drop index surveys_geom_idx");
            migrationBuilder.Sql(@"drop index usersurveys_geom_idx");
            migrationBuilder.Sql(@"delete from cities where upper(name) = 'BETIM' or upper(name) = 'PARÁ DE MINAS'");
        }
    }
}
