using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Models;

namespace Urbiss.Domain.Interfaces
{
    public interface ISurveyFileService
    {
        IEnumerable<SurveyFile> GetFiles();
    }

    public interface ISurveyFileValidateAndProcess
    {
        void Validate(IServiceProvider serviceProvider, string fileName, Geometry area, SurveyConfigDto config);
        void Process(IServiceProvider serviceProvider, Survey survey, string fileName, Geometry area, SurveyConfigDto config);
    }

    public interface ISurveyFileRequestData
    {
        void Process(IServiceProvider serviceProvider, Order order, string fileName, string outputFileName, string csvFileName, Geometry area, SurveyConfigDto config);
    }
}
