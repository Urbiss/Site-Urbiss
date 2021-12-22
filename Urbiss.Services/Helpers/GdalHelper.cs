using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using OSGeo.GDAL;
using System;

namespace Urbiss.Services.Helpers
{
    public static class GdalHelper
    {
        public static Polygon GetRasterMbr(Dataset dataset)
        {
            double[] geotransform = new double[6];

            dataset.GetGeoTransform(geotransform);

            var ulX = geotransform[0];
            var ulY = geotransform[3];
            var xRes = geotransform[1];
            var yRes = geotransform[5];

            var lrX = ulX + dataset.RasterXSize * xRes;
            var lrY = ulY + dataset.RasterYSize * yRes;

            Coordinate upperLeft = new Coordinate(ulX, ulY);
            Coordinate lowerRight = new Coordinate(lrX, lrY);
            return NtsHelper.CreateMbr(upperLeft, lowerRight);
        }
    }
}
