using AspNetCoreHero.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Interfaces;

namespace Urbiss.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : UrbissControllerBase<VoucherController>
    {
        private readonly IVoucherService _voucherService;
        public VoucherController(IVoucherService voucherService)
        {
            this._voucherService = voucherService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IResult> Create([FromBody] VoucherCreateDto model)
        {
            await _voucherService.Create(model);
            return Result.Success("Vouchers solicitados. Será enviado um e-mail para o destinatário!");
        }

        [HttpPost("sendmail")]
        [Authorize(Roles = "Admin")]
        public async Task<IResult> SendMail([FromBody] VoucherSendMail model)
        {
            await _voucherService.SendVouchersMail(model);
            return Result.Success();
        }
    }
}
