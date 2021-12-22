using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Urbiss.Domain.Constants;
using Urbiss.Domain.Interfaces;
using Urbiss.Domain.Models;

namespace Urbiss.Repository
{
    public class AerialPhotoRepository : GenericRepository<AerialPhoto>, IAerialPhotoRepository
    {
        public AerialPhotoRepository(UrbissDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AerialPhoto>> FindByArea(long surveyId, Geometry area)
        {
            var photosInside = await (from ap in _context.AerialPhotos
                                where ((ap.SurveyId == surveyId) && (area.Contains(ap.Geometry)))
                                select ap).ToListAsync();
            //O FromSqlInterpolated não digere muito bem o parâmetro da geometria (AsText)
            var photosOutside = await _context.AerialPhotos.FromSqlRaw(
                    @"select ap.id, ap.name, ap.date, ap.geometry, ap.surveyid, ap.usercreation, ap.creation, ap.modification, ap.usermodification
                      from aerialphotos ap,
                           (select st_geomfromtext('" + area.AsText() + @"', {0}) geom) selecao
                       where ap.surveyid = {1} and
                             not st_contains(selecao.geom, ap.geometry)
                       order by ap.geometry <->selecao.geom
                       limit {2}", SurveyConsts.TARGET_SRID, surveyId, GlobalConsts.MAX_PHOTOS).ToListAsync();
            return photosInside.Union(photosOutside);
        }
    }
}
