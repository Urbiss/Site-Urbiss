using AspNetCoreHero.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Interfaces;

namespace Urbiss.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyController : UrbissControllerBase<SurveyController>
    {
        private readonly ISurveyService _surveyService;

        public SurveyController(ISurveyService surveyService)
        {
            this._surveyService = surveyService;
        }

        [HttpPost("list")]
        public async Task<Result<SurveyListDto>> List([FromBody] WktDto model)
        {
            var list = await _surveyService.List(model, CurrentUserId);
            return Result<SurveyListDto>.Success(list, "Levantamentos disponíveis");
        }

        [HttpPost("listbounds")]
        public async Task<Result<IEnumerable<SurveyBoundsDto>>> ListBounds([FromBody] WktDto model)
        {
            var list = await _surveyService.ListBounds(model);
            return Result<IEnumerable<SurveyBoundsDto>>.Success(list, "Levantamentos disponíveis");
        }

        [HttpPost("process")]
        [Authorize(Roles = "Surveyor,Admin")]
        public async Task<Result<long>> Process([FromBody] SurveyProcessDto model)
        {
            long idSurvey = await _surveyService.Process(new SurveyProcessServiceDto
            { 
                Folder = model.Folder,
                UserId = CurrentUserId 
            });
            return Result<long>.Success(idSurvey, "Levantamento processado");
        }

        /*[DisableFormValueModelBinding]
        [RequestSizeLimit(10L * 1024L * 1024L * 1024L)] 
        [RequestFormLimits(MultipartBodyLengthLimit = 10L * 1024L * 1024L * 1024L)]
        [HttpPost("upload")]
        public async Task<IActionResult> Upload()
        {
            return BadRequest();
        }*/
    }

    /*[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DisableFormValueModelBindingAttribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var factories = context.ValueProviderFactories;
            factories.RemoveType<FormValueProviderFactory>();
            factories.RemoveType<FormFileValueProviderFactory>();
            factories.RemoveType<JQueryFormValueProviderFactory>();
        }
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }*/
}
