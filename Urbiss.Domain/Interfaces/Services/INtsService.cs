using NetTopologySuite.Geometries;
using System.Threading.Tasks;

namespace Urbiss.Domain.Interfaces
{
    public interface INtsService
    {
        Task<Geometry> Transform(Geometry geom, int newSrid);
        Task<double> CalculateGeographyArea(Geometry geom);
    }
}
