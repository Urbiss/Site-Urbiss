using AspNetCoreHero.Results;
using System.Collections.Generic;
using System.Threading.Tasks;
using Urbiss.Domain.Dtos;

namespace Urbiss.Domain.Interfaces
{
    public interface ISurveyService
    {
        Task<IEnumerable<SurveyBoundsDto>> ListBounds(WktDto wkt);
        Task<SurveyListDto> List(WktDto wkt, int userId);
        Task<long> Process(SurveyProcessServiceDto process);
    }
}
