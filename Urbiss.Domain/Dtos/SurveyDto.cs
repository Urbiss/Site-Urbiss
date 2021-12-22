using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using Urbiss.Domain.Converters;

namespace Urbiss.Domain.Dtos
{
    public class SurveyProcessDto
    {
        public string Folder { get; set; }
    }

    public class SurveyProcessServiceDto : SurveyProcessDto
    {
        public int UserId { get; set; }
    }

    public class SurveyServiceBackgroundRequestDto
    {
        public string Folder { get; set; }
        public string OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Wkt { get; set; }
        public int Srid { get; set; }
        public string Email { get; set; }
    }

    public class SurveyProductDto
    {
        public SurveyProductEnum Product { get; set; }
        public string ProductName { get; set; }
    }

    public class SurveyBoundsDto
    {
        public long SurveyId { get; set; }
        public string Wkt { get; set; }
    }

    public class SurveyDto
    {
        public long SurveyId { get; set; }
        public DateTime SurveyDate { get; set; }
        public double Price { get; set; }
        public IEnumerable<SurveyProductDto> Products { get; set; }
    }

    public class SurveyListDto
    {
        public long UserSurveyId { get; set; }
        public double Area { get; set; }
        public IEnumerable<SurveyDto> Surveys { get; set; }
    }

    [JsonObject(ItemRequired=Required.Always)]
    public class SurveyConfigDto
    {
        public string Wkt;
        public int Srid;
        public string LevelColumn;
        [JsonConverter(typeof(DateTimeFormatConverter), "dd/MM/yyyy")]
        public DateTime SurveyDate;
    }

    public enum SurveyFileTypeEnum
    {
        Raster,
        Vector,
        Photo,
        SingleFile
    }

    public enum SurveyProductEnum
    {
        Dtm,
        Dsm,
        OrthoPhoto,
        Contours,
        AerialPhoto,
        UrbanAnalysis
    }

    public enum SurveyStorageTypeEnum
    {
        File,
        Folder
    }
}
