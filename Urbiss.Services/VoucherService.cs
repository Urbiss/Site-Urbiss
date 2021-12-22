using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Exceptions;
using Urbiss.Domain.Interfaces;
using Urbiss.Domain.Models;
using Urbiss.Services.Helpers;

namespace Urbiss.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepository _repoVoucher;
        private readonly IUserService _userService;
        private readonly ILogger<VoucherService> _logger;
        private readonly ISendMailService _sendMailService;

        public VoucherService(IVoucherRepository repoVoucher, IUserService userService, ILogger<VoucherService> logger, ISendMailService sendMailService)
        {
            this._repoVoucher = repoVoucher;
            this._userService = userService;
            this._logger = logger;
            this._sendMailService = sendMailService;
        }

        private static string FormatVoucher(string code)
        {
            return $"{code.Substring(0,4)}-{code.Substring(4, 4)}-{code.Substring(8, 4)}";
        }

        private static string GenerateVoucherCode()
        {
            string key = StringHelper.GenerateRandomString(8).ToUpper();
            string code = $"{DateTime.Now.Year}{key.Substring(0, 4)}{key.Substring(4, 4)}";
            return FormatVoucher(code);
        }

        public async Task Create(VoucherCreateDto voucher)
        {
            if (await _userService.IsAdmin(_userService.CurrentUserId))
            {
                if (voucher.Quantity <= 0)
                    throw new ApiException("Quantidade inválida ou não informada!");
                if (voucher.Area <= 0)
                    throw new ApiException("Área inválida ou não informada!");
                if (voucher.Days <= 0)
                    throw new ApiException("Número de dias inválido ou não informado!");
                using var transaction = _repoVoucher.Context.Database.BeginTransaction();
                try
                {
                    for (int i = 0; i < voucher.Quantity; i++)
                    {
                        var newVoucher = new Voucher
                        {
                            Code = GenerateVoucherCode(),
                            Area = voucher.Area,
                            Email = voucher.Email.ToLower(),
                            Expiration = DateTime.Now.AddDays(voucher.Days).Date,
                            Status = VoucherStatusEnum.Pending
                        };
                        await _repoVoucher.Create(newVoucher);
                    }
                    // Commit transaction if all commands succeed, transaction will auto-rollback
                    // when disposed if either commands fails
                    transaction.Commit();
                    await SendVouchersMail(new VoucherSendMail
                    {
                        Email = voucher.Email
                    });
                }
                catch (Exception exc)
                {
                    _logger.LogError(exc.Message, exc);
                    throw new ApiException("Não foi possível gerar os vouchers!");
                }
            }
            else
                throw new ApiException("Usuário não possui permissão para criar voucher!");
        }

        private const string VOUCHER_MAIL_TEMPLATE = @"
            <font size=""2"" face=""Verdana"">
            <table width=""700"" border=""0"" align=""center"" cellpadding=""0"" cellspacing=""0"">
                <tr>
                    <td>
                        <div align=""center""><img src=""http://urbiss.com.br/wp-content/uploads/2021/03/logo-urbis-2.png"" width=""333"" height=""90"" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>            
                <tr>
                    <td>Lista de vouchers emitidos para o e-mail: {0}
                        <br/>
                        <br/>
                    </td>
                </tr>            
                <tr>
                    <td><font size=""3"" face=""Courier"">{1}</font><br/><br/></td>
                </tr>
                <tr>
                    <td><p align=""center"">Mensagem automática enviada pela Urbiss (não responda) <br /><td/>
                <tr/>
            </table>
        ";

        public async Task SendVouchersMail(VoucherSendMail voucherSendMail)
        {
            var vouchers = await _repoVoucher.ListValidsByEmail(voucherSendMail.Email);
            if (vouchers.Any())
            {
                var voucherHtmlList = vouchers.Select(v => $"    {v.Code}&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Expira em: {v.Expiration:dd/MM/yyyy}&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Área máxima: {v.Area:N2}");
                var body = string.Format(VOUCHER_MAIL_TEMPLATE, voucherSendMail.Email, string.Join("<br/>", voucherHtmlList));
                var mailRequest = new MailRequestDto
                {
                    To = voucherSendMail.Email,
                    Subject = $"Vouchers",
                    Body = body
                };
                _sendMailService.Send(mailRequest);
            }
        }

        public async Task<long> VoucherIsValid(string voucherCode, double area)
        {
            if (string.IsNullOrEmpty(voucherCode))
                throw new ApiException("Código de voucher não informado!");
            if (voucherCode.Length == 12)
                voucherCode = FormatVoucher(voucherCode);
            if (voucherCode.Length != 14)
                throw new ApiException("Código de voucher inválido!");
            var voucher = await _repoVoucher.FindByCode(voucherCode);
            if (voucher == null)
                throw new ApiException("Código de voucher não encontrado!");
            if (voucher.Status != VoucherStatusEnum.Pending)
                throw new ApiException("Código de voucher cancelado ou já utilizado!");
            if (DateTime.Now.Date > voucher.Expiration.Date)
                throw new ApiException("Código de voucher expirado!");
            if (voucher.Area < area)
                throw new ApiException("A área selecionada é maior do que a área do voucher!");
            return voucher.Id;
        }

        public async Task UseVoucher(long id)
        {
            var voucher = await _repoVoucher.FindById(id);
            voucher.Status = VoucherStatusEnum.Used;
            await _repoVoucher.Update(voucher);
        }
    }
}
