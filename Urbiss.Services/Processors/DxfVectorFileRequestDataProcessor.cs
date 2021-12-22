using netDxf;
using netDxf.Entities;
using netDxf.Tables;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.IO;
using Urbiss.Domain.Constants;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Interfaces;
using Urbiss.Domain.Models;

namespace Urbiss.Services.Processors
{
    public class DxfVectorFileRequestDataProcessor : ISurveyFileRequestData
    {
        public void Process(IServiceProvider serviceProvider, Order order, string fileName, string outputFileName, string csvFileName, Geometry area, SurveyConfigDto config)
        {
            outputFileName = Path.ChangeExtension(outputFileName, "dxf");
            DxfDocument doc = new DxfDocument();

            using (var sfdr = new ShapefileDataReader(fileName, GeometryFactory.Default))
            {
                var header = sfdr.DbaseHeader;
                var levelColumnIndex = sfdr.GetOrdinal(config.LevelColumn);

                var areaLayer = new Layer(SurveyConsts.DXF_LAYER_AREA);
                var line2dLayer = new Layer(SurveyConsts.DXF_LAYER_LEVEL_2D);
                var line3dLayer = new Layer(SurveyConsts.DXF_LAYER_LEVEL_3D);
                var labelLayer = new Layer(SurveyConsts.DXF_LAYER_LABEL);

                Vector3[] vertex = Array.ConvertAll<Coordinate, Vector3>(area.Coordinates, (Coordinate c) => new Vector3(c.X, c.Y, 0));
                Polyline areaEntity = new Polyline(vertex, true)
                {
                    Layer = areaLayer
                };
                doc.AddEntity(areaEntity);

                while (sfdr.Read())
                {
                    var lineIntersection = sfdr.Geometry.Intersection(area);
                    if (lineIntersection != null)
                    {
                        var level = sfdr.GetDouble(levelColumnIndex);
                        for (int i = 0; i < (lineIntersection.Coordinates.Length - 1); i++)
                        {
                            var startPoint = lineIntersection.Coordinates[i];
                            var endPoint = lineIntersection.Coordinates[i + 1];
                            Line entity2D = new Line(new Vector2(startPoint.X, startPoint.Y), new Vector2(endPoint.X, endPoint.Y))
                            {
                                Layer = line2dLayer
                            };

                            Line entity3D = new Line(new Vector3(startPoint.X, startPoint.Y, level), new Vector3(endPoint.X, endPoint.Y, level))
                            {
                                Layer = line3dLayer
                            };

                            if ((i == 0) || (i == lineIntersection.Coordinates.Length - 1))
                            {
                                NetTopologySuite.Geometries.Point labelGeom = GeometryFactory.Default.CreateLineString(new Coordinate[] { lineIntersection.Coordinates[i], lineIntersection.Coordinates[i + 1] }).Centroid;
                                MText label = new MText(level.ToString(), new Vector2(labelGeom.X, labelGeom.Y), 1, 1)
                                {
                                    Layer = labelLayer
                                };
                                doc.AddEntity(label);
                            }

                            doc.AddEntity(entity2D);
                            doc.AddEntity(entity3D);
                        }
                    }
                }
                sfdr.Close();
            }
            doc.Save(outputFileName);
        }
    }
}
