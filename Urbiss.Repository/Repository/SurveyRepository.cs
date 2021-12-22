using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Urbiss.Domain.Constants;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Interfaces;
using Urbiss.Domain.Models;

namespace Urbiss.Repository
{
    public class SurveyRepository : GenericRepository<Survey>, ISurveyRepository
    {
        private readonly ILogger<ISurveyRepository> _logger;
        private readonly AppSettingsDto _appSettings;
        private readonly ISurveyFileService _surveyFileService;

        public SurveyRepository(UrbissDbContext context, ILogger<ISurveyRepository> logger, IOptions<AppSettingsDto> appSettings, ISurveyFileService surveyFileService) : base(context) 
        {
            this._appSettings = appSettings.Value;
            this._surveyFileService = surveyFileService;
            this._logger = logger;
        }

        public IEnumerable<SurveyProductDto> GetSurveyProducts(string folder)
        {
            var result = new List<SurveyProductDto>();
            var surveyFolderName = AppSettingsDto.GetAppFolder(_appSettings.DataFolder, folder);
            foreach (var file in _surveyFileService.GetFiles())
            {
                bool add = false;

                if (file.StorageType == SurveyStorageTypeEnum.File)
                    add = File.Exists(Path.Combine(surveyFolderName, file.FileName));
                if (file.StorageType == SurveyStorageTypeEnum.Folder)
                    add = Directory.Exists(Path.Combine(surveyFolderName, file.FileName));

                if (add)
                {
                    result.Add(new SurveyProductDto
                    {
                        Product = file.Product,
                        ProductName = file.ProductName
                    });
                }
            }
            return result;
        }

        public async Task<IEnumerable<SurveyDto>> FindByArea(Geometry geometry)
        {
            var surveys = await _dataset.Where(s => s.Geometry.Contains(geometry)).ToListAsync();
            //Verificando os produtos do levantamento
            var result = new List<SurveyDto>();
            foreach(var survey in surveys)
            {
                result.Add(new SurveyDto 
                { 
                    SurveyId = survey.Id,
                    SurveyDate = survey.SurveyDate,
                    Products = GetSurveyProducts(survey.Folder)
                });
            }
            return result;
        }

        public async Task<bool> FolderExists(string name)
        {
            return await _dataset
                         .Where(s => s.Folder == name.ToUpper())
                         .AnyAsync();
        }

        public async Task<IEnumerable<SurveyBoundsDto>> FindByBounds(Geometry geometry)
        {
            var surveys = await _dataset.Where(s => geometry.Intersects(s.Geometry)).ToListAsync(); 
            //Verificando os produtos do levantamento
            var result = new List<SurveyBoundsDto>();
            foreach (var survey in surveys)
            {
                result.Add(new SurveyBoundsDto
                {
                    SurveyId = survey.Id,
                    Wkt = survey.Geometry.AsText()
                });
            }
            return result;
        }
    }
}
