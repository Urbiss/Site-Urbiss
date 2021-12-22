using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Urbiss.Domain.Constants;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Exceptions;
using Urbiss.Domain.Interfaces;
using Urbiss.Domain.Models;
using Urbiss.Services.Helpers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Urbiss.Services
{
    public class SurveyService : ISurveyService
    {
        private readonly ISurveyRepository _repoSurvey;
        private readonly AppSettingsDto _appSettings;
        private readonly IUserSurveyRepository _repoUserSurvey;
        private readonly INtsService _ntsService;
        private readonly ISurveyFileService _surveyFileService;
        private readonly IHttpContextAccessor _context;
        public readonly IServiceProvider _serviceProvider;

        public SurveyService(ISurveyRepository repoSurvey, IOptions<AppSettingsDto> appSettings, ISurveyFileService surveyFileService,
            IUserSurveyRepository repoUserSurvey, INtsService ntsService, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            this._repoSurvey = repoSurvey;
            this._appSettings = appSettings.Value;
            this._repoUserSurvey = repoUserSurvey;
            this._ntsService = ntsService;
            this._surveyFileService = surveyFileService;
            this._context = httpContextAccessor;
            this._serviceProvider = serviceProvider;
        }

        public async Task<SurveyListDto> List(WktDto wkt, int userId)
        {
            var area = NtsHelper.CreatePolygonFromWkt(wkt.Wkt, wkt.Srid);
            if (!area.IsValid)
                throw new ApiException("Geometria inválida!");

            var targetArea = await _ntsService.Transform(area, SurveyConsts.TARGET_SRID);

            var surveys = await _repoSurvey.FindByArea(targetArea);
            var logUser = new UserSurvey
            {
                UserId = userId,
                Date = DateTime.Now,
                Geometry = targetArea as Polygon
            };

            logUser.Ip = _context.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            logUser.Area = await _ntsService.CalculateGeographyArea(logUser.Geometry);
            var userArea = await _repoUserSurvey.Create(logUser);
            // TODO: Calcular o preço da área
            foreach (var survey in surveys)
                survey.Price = 0;//Math.Round(logUser.Area * 2.50, 2);
            var result = new SurveyListDto 
            { 
                UserSurveyId = userArea.Id, 
                Area = logUser.Area,
                Surveys = surveys 
            };
            return result;
        }

        public async Task<IEnumerable<SurveyBoundsDto>> ListBounds(WktDto wkt)
        {
            var area = NtsHelper.CreatePolygonFromWkt(wkt.Wkt, wkt.Srid);
            if (!area.IsValid)
                throw new ApiException("Geometria inválida!");

            var targetArea = await _ntsService.Transform(area, SurveyConsts.TARGET_SRID);

            var boundsArea = await _ntsService.CalculateGeographyArea(targetArea);
            if (boundsArea > _appSettings.MaxAreaBounds)
                throw new ApiException("A área pesquisada é muito grande! Aumente o zoom e tente novamente.");

            var surveys = await _repoSurvey.FindByBounds(targetArea);
            return surveys;
        }

        public async Task<long> Process(SurveyProcessServiceDto process)
        {
            // TODO: o processamento das informações urbanísticas deve ler o shape com metadados da pasta upload, criar uma tabela com o prefixo do código do município e ler dos comentários das colunas o texto a ser exibido como chave do par chave/valor
            // COMMENT ON COLUMN my_table.my_column IS 'Employee ID number'
            /*SELECT c.table_schema,c.table_name,c.column_name,pgd.description
              FROM pg_catalog.pg_statio_all_tables as st
                   inner join pg_catalog.pg_description pgd on(pgd.objoid = st.relid)
                   inner join information_schema.columns c on(pgd.objsubid = c.ordinal_position
                   and  c.table_schema = st.schemaname and c.table_name = st.relname)*/

            if (await _repoSurvey.FolderExists(process.Folder))
                throw new ApiException("A pasta informada já foi processada!");

            var surveyFolderName = AppSettingsDto.GetAppFolder(_appSettings.DataFolder, process.Folder);

            if (!Directory.Exists(surveyFolderName))
                throw new ApiException($"A pasta {process.Folder} não existe!");

            var surveyConfigFile = JsonConvert.DeserializeObject<SurveyConfigDto>(File.ReadAllText(Path.Combine(surveyFolderName, SurveyConsts.CONFIG_FILE)));

            var area = NtsHelper.CreateFromWkt(surveyConfigFile.Wkt, surveyConfigFile.Srid);

            if (!area.IsValid)
                throw new ApiException("Geometria inválida!");

            var files = _surveyFileService.GetFiles();
            
            var productsCount = 0;
            foreach (var file in files)
            {
                var fullPathFileName = Path.Combine(surveyFolderName, file.FileName);
                if ((File.Exists(fullPathFileName)) || (Directory.Exists(fullPathFileName)))
                {
                    file.Validate(this._serviceProvider, fullPathFileName, area, surveyConfigFile);
                    productsCount++;
                }
            }

            if (productsCount == 0)
                throw new ApiException("O levantamento informado não possui nenhum arquivo!");

            var targetArea = await _ntsService.Transform(area, SurveyConsts.TARGET_SRID);
            var survey = new Survey
            {
                Geometry = targetArea as Polygon,
                UserId = process.UserId,
                SurveyDate = surveyConfigFile.SurveyDate,
                Folder = process.Folder,
                Srid = surveyConfigFile.Srid

            };
            var result = await _repoSurvey.Create(survey);

            foreach (var file in files)
                file.Process(_serviceProvider, survey, Path.Combine(surveyFolderName, file.FileName), area, surveyConfigFile);

            return result.Id;
        }
    }
}
