using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Interfaces;
using Urbiss.Domain.Models;

namespace Urbiss.Repository
{
    public class CityRepository : GenericRepository<City>, ICityRepository
    {
        public CityRepository(UrbissDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CityWithoutGeometryDto>> FindAllWithoutGeometry()
        {
            return await _dataset
                         .Select(c => new CityWithoutGeometryDto 
                            { Id = c.Id, 
                              IbgeCode = c.IbgeCode, 
                              Name = c.Name, 
                              Zoom = c.Zoom, 
                              LatitudeCenter = c.LatitudeCenter, 
                              LongitudeCenter = c.LongitudeCenter }).ToListAsync();
        }

        public async Task<IEnumerable<City>> FindByArea(Geometry geometry)
        {
            return await _dataset
                         .Where(c => c.Geometry.Intersects(geometry)).ToListAsync();
        }
    }
}
