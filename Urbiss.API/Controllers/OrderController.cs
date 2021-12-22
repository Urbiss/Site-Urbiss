using AspNetCoreHero.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Interfaces;

namespace Urbiss.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : UrbissControllerBase<OrderController>
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            this._orderService = orderService;
        }

        [HttpGet("list")]
        public async Task<Result<IEnumerable<OrderListDto>>> List()
        {
            var orders = await _orderService.List(CurrentUserId);
            return Result<IEnumerable<OrderListDto>>.Success(orders, "Listagem de pedidos");
        }

        [HttpPost("create")]
        public async Task<IResult> Create([FromBody] OrderDto model)
        {
            var order = new OrderServiceDto
            {
                SurveyId = model.SurveyId,
                UserSurveyId = model.UserSurveyId,
                UserRequestId = CurrentUserId,
                VoucherCode = model.VoucherCode
            };
            await _orderService.Create(order);
            return Result.Success("Requisição efetuada. Será enviado um e-mail quando a requisição for concluída!");
        }

        [HttpGet("download/{orderId}")]
        public async Task<IActionResult> Download(string orderId)
        {
            var filePath = await _orderService.GetFilePathToDownload(orderId, CurrentUserId);
            var stream = new FileStream(filePath, FileMode.Open);
            var result = new FileStreamResult(stream, "application/zip")
            {
                FileDownloadName = Path.GetFileName(filePath),
                EnableRangeProcessing = true
            };
            return result;
        }
    }
}
