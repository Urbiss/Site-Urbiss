using AspNetCoreHero.Results;
using System.Collections.Generic;
using System.Threading.Tasks;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Interfaces;

namespace Urbiss.Services
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _repoCity;

        public CityService(ICityRepository repoCity)
        {
            this._repoCity = repoCity;
        }

        public async Task<IEnumerable<CityWithoutGeometryDto>> List()
        {
            var cities = await _repoCity.FindAllWithoutGeometry();
            return cities;
        }
    }
}
