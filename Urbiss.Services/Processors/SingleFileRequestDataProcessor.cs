using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using System;
using System.IO;
using System.Threading.Tasks;
using Urbiss.Domain.Constants;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Interfaces;
using Urbiss.Domain.Models;

namespace Urbiss.Services.Processors
{
    public class SingleFileRequestDataProcessor : ISurveyFileRequestData
    {
        public void Process(IServiceProvider serviceProvider, Order order, string fileName, string outputFileName, string csvFileName, Geometry area, SurveyConfigDto config)
        {
            if (File.Exists(fileName))
                File.Copy(fileName, outputFileName);
        }
    }
}
