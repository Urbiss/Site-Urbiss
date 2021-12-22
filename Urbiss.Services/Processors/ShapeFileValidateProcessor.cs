using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.IO;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Exceptions;
using Urbiss.Domain.Interfaces;
using Urbiss.Domain.Models;
using Urbiss.Services.Helpers;

namespace Urbiss.Services.Processors
{
    public class ShapeFileValidateProcessor : ISurveyFileValidateAndProcess
    {
        private static void ValidateFileByExtension(string fileName, string extension)
        {
            var projectionFileName = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + $".{extension}");
            if (!File.Exists(projectionFileName))
                throw new ApiException($"O arquivo {projectionFileName} não existe!");
        }

        public void Validate(IServiceProvider servideProvider, string fileName, Geometry area, SurveyConfigDto config)
        {
            ValidateFileByExtension(fileName, "shp");
            ValidateFileByExtension(fileName, "shx");
            ValidateFileByExtension(fileName, "dbf");
            ValidateFileByExtension(fileName, "prj");
            using var vector = new ShapefileDataReader(fileName, GeometryFactory.Default);
            if (!vector.ShapeHeader.Bounds.Intersects(area.EnvelopeInternal))
                throw new ApiException($"O MBR do arquivo {Path.GetFileName(fileName)} não intercepta a área do arquivo de configuração!");
            if (NtsHelper.GetShapefileColumnIndexByName(vector, config.LevelColumn) == -1)
                throw new ApiException($"A coluna {config.LevelColumn} não foi encontrada no arquivo {Path.GetFileName(fileName)}!");
        }

        public void Process(IServiceProvider serviceProvider, Survey survey, string fileName, Geometry area, SurveyConfigDto config)
        {
        }
    }
}
