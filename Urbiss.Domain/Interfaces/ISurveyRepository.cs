using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Threading.Tasks;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Models;

namespace Urbiss.Domain.Interfaces
{
    public interface ISurveyRepository : IGenericRepository<Survey>
    {
        Task<IEnumerable<SurveyDto>> FindByArea(Geometry geometry);
        Task<IEnumerable<SurveyBoundsDto>> FindByBounds(Geometry geometry);
        Task<bool> FolderExists(string name);
    }
}
