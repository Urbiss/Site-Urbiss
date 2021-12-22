using NetTopologySuite.Geometries;
using System.Threading.Tasks;
using Urbiss.Domain.Models;

namespace Urbiss.Domain.Interfaces
{
    public interface IDatabaseUtilsRepository
    {
        Task<SpatialReferenceSystem> FindSridById(int srid);
        Task<double> CalculateGeographyArea(Geometry geometry);
    }

}
