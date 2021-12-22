using AspNetCoreHero.Results;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Interfaces;

namespace Urbiss.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : UrbissControllerBase<CityController>
    {
        private readonly ICityService _cityService;
        public CityController(ICityService cityService)
        {
            this._cityService = cityService;
        }

        [HttpGet("list")]
        public async Task<Result<IEnumerable<CityWithoutGeometryDto>>> List()
        {
            var cities = await _cityService.List();
            return Result<IEnumerable<CityWithoutGeometryDto>>.Success(cities, "Cidades disponíveis");
        }
    }
}
