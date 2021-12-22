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
    public class AerialPhotoRequestDataProcessor : ISurveyFileRequestData
    {
        public void Process(IServiceProvider serviceProvider, Order order, string fileName, string outputFileName, string csvFileName, Geometry area, SurveyConfigDto config)
        {
            using var scope = serviceProvider.CreateScope();
            IAerialPhotoRepository repo = scope.ServiceProvider.GetRequiredService<IAerialPhotoRepository>();
            INtsService ntsService = scope.ServiceProvider.GetRequiredService<INtsService>();
            var geomWgs = ntsService.Transform(area, SurveyConsts.TARGET_SRID).Result;
            int idPhoto = 0;
            var photos = repo.FindByArea(order.SurveyId, geomWgs).Result;
            foreach (var photo in photos)
            {
                var sourceFileName = Path.Combine(fileName, photo.Name);
                var photoOutputFileName = string.Format(outputFileName, ++idPhoto);
                photoOutputFileName += Path.GetExtension(sourceFileName);
                File.Copy(sourceFileName, photoOutputFileName, true);
            }
        }
    }
}
