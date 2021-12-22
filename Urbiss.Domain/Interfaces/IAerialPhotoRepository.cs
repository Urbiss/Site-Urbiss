using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Threading.Tasks;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Models;

namespace Urbiss.Domain.Interfaces
{
    public interface IAerialPhotoRepository : IGenericRepository<AerialPhoto>
    {
        Task<IEnumerable<AerialPhoto>> FindByArea(long surveyId, Geometry area);
    }
}
