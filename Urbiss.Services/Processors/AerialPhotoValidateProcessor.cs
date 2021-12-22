using ExifLib;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.IO;
using Urbiss.Domain.Constants;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Exceptions;
using Urbiss.Domain.Interfaces;
using Urbiss.Domain.Models;
using Urbiss.Services.Helpers;

namespace Urbiss.Services.Processors
{
    public class AerialPhotoValidateProcessor : ISurveyFileValidateAndProcess
    {
        private readonly List<AerialPhoto> photos = new List<AerialPhoto>();

        public void Validate(IServiceProvider serviceProvider, string fileName, Geometry area, SurveyConfigDto config)
        {
            foreach (var file in Directory.GetFiles(fileName))
            {
                var extension = Path.GetExtension(file).Replace(".", "").ToLower();
                if ((extension.Equals("jpg")) || (extension.Equals("jpeg")))
                {
                    var info = ImageHelper.ReadExifData(file);
                    if (info != null)
                    {
                        var newPhoto = new AerialPhoto
                        {
                            Geometry = NtsHelper.CreatePoint(info.Latitude, info.Longitude, SurveyConsts.TARGET_SRID),
                            Date = info.DateTime,
                            Name = Path.GetFileName(file),
                        };
                        photos.Add(newPhoto);
                    }
                }
            }
            if (photos.Count == 0)
                throw new ApiException("A pasta fotos está vazia!"); 
        }

        public async void Process(IServiceProvider serviceProvider, Survey survey, string fileName, Geometry area, SurveyConfigDto config)
        {
            using var scope = serviceProvider.CreateScope();
            IAerialPhotoRepository repo = scope.ServiceProvider.GetRequiredService<IAerialPhotoRepository>();
            foreach(var photo in photos)
            {
                photo.SurveyId = survey.Id;
                await repo.Create(photo);
            }
        }
    }
}
