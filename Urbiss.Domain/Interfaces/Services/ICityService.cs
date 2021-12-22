using System.Collections.Generic;
using System.Threading.Tasks;
using Urbiss.Domain.Dtos;

namespace Urbiss.Domain.Interfaces
{
    public interface ICityService
    {
        Task<IEnumerable<CityWithoutGeometryDto>> List();
    }
}
