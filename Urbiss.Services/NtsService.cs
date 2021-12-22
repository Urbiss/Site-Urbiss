using Microsoft.Extensions.Caching.Memory;
using NetTopologySuite.Geometries;
using System;
using System.Threading.Tasks;
using Urbiss.Domain.Interfaces;
using Urbiss.Services.Helpers;

namespace Urbiss.Services
{
    public class NtsService : INtsService
    {
        private readonly IDatabaseUtilsRepository _repoDbUtils;
        private readonly IMemoryCache _memoryCache;

        public NtsService(IDatabaseUtilsRepository repoDbUtils, IMemoryCache memoryCache)
        {
            this._repoDbUtils = repoDbUtils;
            this._memoryCache = memoryCache;
        }

        private static string GetCacheKey(int srid)
        {
            return $"TransformGeometryService-{srid}";
        }

        private async Task<string> GetProjectionTextBySrid(int srid)
        {
            //https://balta.io/blog/aspnet-memory-cache
            string cacheKey = GetCacheKey(srid);
            string result = await _memoryCache.GetOrCreateAsync<string>(cacheKey, async entry =>
            {
                var srs = await _repoDbUtils.FindSridById(srid);
                entry.Size = 1;
                return srs.Text;

            });
            return result;
        }

        public async Task<Geometry> Transform(Geometry geom, int newSrid)
        {
            string sourceSridText = await GetProjectionTextBySrid(geom.SRID);
            string targetSridText = await GetProjectionTextBySrid(newSrid);
            return TransformGeometryHelper.ProjectGeometry(geom, sourceSridText, targetSridText);
        }

        public async Task<double> CalculateGeographyArea(Geometry geom)
        {
            return await _repoDbUtils.CalculateGeographyArea(geom);
        }
    }
}
