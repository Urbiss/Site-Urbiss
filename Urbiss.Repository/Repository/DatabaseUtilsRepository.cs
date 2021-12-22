using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Threading.Tasks;
using Urbiss.Domain.Interfaces;
using Urbiss.Domain.Models;

namespace Urbiss.Repository
{
    public class DatabaseUtilsRepository : IDatabaseUtilsRepository
    {
        protected readonly UrbissDbContext _context;
        protected readonly DbSet<SpatialReferenceSystem> _dataset;
        public DatabaseUtilsRepository(UrbissDbContext context)
        {
            this._context = context;
            this._context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            this._dataset = _context.Set<SpatialReferenceSystem>();
        }

        public async Task<double> CalculateGeographyArea(Geometry geometry)
        {
            //Atualizando a área. O NetTopologySuite não possui um método para calcular a área geodésica.
            //Por isso, atualizo utilizando a função do postgis
            return await this._context.Database.GetDbConnection().ExecuteScalarAsync<double>(0.0, @"select st_area(st_geomfromtext(:wkt, :srid) ::geography)", 
                "wkt", geometry.AsText(), "srid", geometry.SRID);
        }

        public async Task<SpatialReferenceSystem> FindSridById(int srid)
        {
            return await _dataset.SingleOrDefaultAsync(sr => sr.Srid.Equals(srid));
        }
    }
}
