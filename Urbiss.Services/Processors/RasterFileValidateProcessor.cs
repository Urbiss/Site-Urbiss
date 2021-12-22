using MaxRev.Gdal.Core;
using NetTopologySuite.Geometries;
using OSGeo.GDAL;
using OSGeo.OSR;
using System;
using System.IO;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Exceptions;
using Urbiss.Domain.Interfaces;
using Urbiss.Domain.Models;
using Urbiss.Services.Helpers;

namespace Urbiss.Services.Processors
{
    public class RasterFileValidateProcessor : ISurveyFileValidateAndProcess
    {
        public void Process(IServiceProvider serviceProvider, Survey survey, string fileName, Geometry area, SurveyConfigDto config)
        {
        }

        public void Validate(IServiceProvider servideProvider, string fileName, Geometry area, SurveyConfigDto config)
        {
            GdalBase.ConfigureAll();
            using var raster = Gdal.Open(fileName, Access.GA_ReadOnly);
            var proj = new SpatialReference(raster.GetProjection());
            var srid = Convert.ToInt32(proj.GetAttrValue("AUTHORITY", 1));
            if (area.SRID != srid)
                throw new ApiException($"O srid do arquivo {Path.GetFileName(fileName)} é diferente do srid do arquivo de configuração!");
            Polygon mbr = GdalHelper.GetRasterMbr(raster);
            if (!mbr.Contains(area))
                throw new ApiException(message: $"O MBR do arquivo {Path.GetFileName(fileName)} não contém a área do arquivo de configuração!");
        }
    }
}
