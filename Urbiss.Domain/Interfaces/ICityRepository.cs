using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Threading.Tasks;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Models;

namespace Urbiss.Domain.Interfaces
{
    public interface ICityRepository : IGenericRepository<City>
    {
        Task<IEnumerable<CityWithoutGeometryDto>> FindAllWithoutGeometry();
        Task<IEnumerable<City>> FindByArea(Geometry geometry);
    }
}
