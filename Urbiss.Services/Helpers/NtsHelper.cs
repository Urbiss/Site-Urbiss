using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Threading.Tasks;
using Urbiss.Domain.Interfaces;
using Urbiss.Domain.Models;

namespace Urbiss.Services.Helpers
{
    public static class NtsHelper
    {
        public static Geometry CreateFromWkt(string wkt, int srid)
        {
            var reader = new WKTReader
            {
                DefaultSRID = srid
            };
            return reader.Read(wkt);
        }

        public static Polygon CreatePolygonFromWkt(string wkt, int srid)
        {
            if (string.IsNullOrEmpty(wkt))
                throw new Exception("WKT não informado!");
            var polygon = NtsHelper.CreateFromWkt(wkt, srid);
            if (polygon.OgcGeometryType != OgcGeometryType.Polygon)
                throw new Exception("WKT não é um polígono!");
            if (!polygon.IsValid)
                throw new Exception("WKT inválido!");
            return (polygon as Polygon);
        }

        public static Point CreatePoint(double latitude, double longitude, int srid)
        {
            Coordinate c = new Coordinate(longitude, latitude);
            var result = GeometryFactory.Default.CreatePoint(c);
            result.SRID = srid;
            return result;
        }

        public static Polygon CreateMbr(Coordinate upperLeft, Coordinate lowerRight)
        {
            var coords = new Coordinate[]
            {
                upperLeft,
                new Coordinate(lowerRight.X, upperLeft.Y),
                lowerRight,
                new Coordinate(upperLeft.X, lowerRight.Y),
                upperLeft
            };
            return GeometryFactory.Default.CreatePolygon(coords);
        }

        public static int GetShapefileColumnIndexByName(ShapefileDataReader shape, string columnName)
        {
            columnName = columnName.ToUpper();
            for(int i = 0; i < shape.DbaseHeader.NumFields; i++)
            {
                if (shape.DbaseHeader.Fields[i].Name.ToUpper().Equals(columnName))
                    return i;
            }
            return -1;
        }

        public static double ConvertLatLongToDecimal(double[] value)
        {
            return value[0] + value[1] / 60 + value[2] / 3600;
        }
    }
}
