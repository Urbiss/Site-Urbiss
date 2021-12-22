using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Urbiss.Domain.Constants;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Exceptions;
using Urbiss.Domain.Interfaces;
using Urbiss.Domain.Models;
using Urbiss.Services.Helpers;

namespace Urbiss.Services
{
    public class OrderRequestProductBackgroundService : IOrderRequestProductBackgroundService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly ISendMailService _mailService;
        private readonly ICityRepository _repoCity;
        private readonly IUserSurveyRepository _repoUserSurvey;
        private readonly ISurveyRepository _repoSurvey;
        private readonly INtsService _ntsService;
        private readonly AppSettingsDto _appSettings;
        private readonly IUserService _userService;
        private readonly IOrderRepository _repoOrder;
        private readonly ISurveyFileService _surveyFileService;
        private readonly IVoucherService _voucherService;
        private readonly IServiceProvider _serviceProvider;

        public OrderRequestProductBackgroundService(ILogger<OrderService> logger, IOptions<AppSettingsDto> appSettings, ISurveyFileService surveyFileService,
            ISendMailService mailService, ICityRepository repoCity, IUserSurveyRepository repoUserSurvey, ISurveyRepository repoSurvey, INtsService ntsService,
            IUserService userService, IOrderRepository repoOrder, IVoucherService voucherService, IServiceProvider serviceProvider)
        {
            this._logger = logger;
            this._repoCity = repoCity;
            this._repoUserSurvey = repoUserSurvey;
            this._repoSurvey = repoSurvey;
            this._ntsService = ntsService;
            this._appSettings = appSettings.Value;
            this._userService = userService;
            this._mailService = mailService;
            this._repoOrder = repoOrder;
            this._surveyFileService = surveyFileService;
            this._voucherService = voucherService;
            this._serviceProvider = serviceProvider;
        }

        private const string ORDER_MAIL_TEMPLATE = @"
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
                    <td style=""text-align:center"">O pedido de código [{0}] solicitado em {1} foi processado com sucesso. <p>Para efetuar o download dos produtos, clique no link abaixo:</p>
                    </td>
                </tr>            
                <tr>
                    <td>&nbsp;</td>
                </tr>            
                <tr>
                    <td style=""text-align:center""><a href='{2}'>Download dos produtos</a>
                    </td
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>            
                <tr>
                    <td><p align=""center"">Mensagem automática enviada pela Urbiss (não responda) <br /><td/>
                <tr/>
            </table>
        ";

        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new int[] { 300 })]
        [Queue(HangfireConsts.ORDER_REQUEST_PRODUCT_QUEUE)]
        public void RequestProducts(SurveyServiceBackgroundRequestDto request)
        {
            var surveyFolderName = AppSettingsDto.GetAppFolder(_appSettings.DataFolder, request.Folder);
            var surveyConfigFile = JsonConvert.DeserializeObject<SurveyConfigDto>(File.ReadAllText(Path.Combine(surveyFolderName, SurveyConsts.CONFIG_FILE)));

            var geometry = NtsHelper.CreateFromWkt(request.Wkt, request.Srid);

            var outputFolderName = AppSettingsDto.GetAppFolder(_appSettings.OutputFolder, request.OrderId);

            if (Directory.Exists(outputFolderName))
                Directory.Delete(outputFolderName, true);

            Directory.CreateDirectory(outputFolderName);
            var csvFileName = Path.Combine(outputFolderName, "wkt.csv");
            File.WriteAllText(csvFileName, $"id,wkt{Environment.NewLine}1,\"{geometry.AsText()}\"");

            var order = _repoOrder.FindByOrderId(request.OrderId).Result;

            foreach (var file in _surveyFileService.GetFiles())
            {
                var fullPathFileName = Path.Combine(surveyFolderName, file.FileName);
                var fullPathOutputFileName = Path.Combine(outputFolderName, file.OutputFileName);
                if ((File.Exists(fullPathFileName)) || (Directory.Exists(fullPathFileName)))
                    file.RequestData(_serviceProvider, order, fullPathFileName, fullPathOutputFileName, csvFileName, geometry, surveyConfigFile);
            }

            File.Delete(csvFileName);

            var zipTempFileName = AppSettingsDto.GetAppFolder(_appSettings.TempFolder, $"{request.OrderId}.zip");
            ZipFile.CreateFromDirectory(outputFolderName, zipTempFileName);

            Array.ForEach(Directory.GetFiles(outputFolderName), file => File.Delete(file));

            var zipFileName = Path.Combine(outputFolderName, $"{request.OrderId}.zip");
            File.Move(zipTempFileName, zipFileName);

            File.Delete(zipTempFileName);

            //Atualizando o status da ordem
            //Os métodos são assíncronos mas invoco de modo síncrono por causa do hangfire não aceitar um método async no enqueue (na versão 2.0 do hangfire isso será possível)
            order.Status = OrderStatusEnum.Processed;
            _repoOrder.Update(order).Wait();

            var url = HttpUtility.HtmlEncode($"{ _appSettings.Url }/order/{request.OrderId}/download");
            var mailRequest = new MailRequestDto
            {
                To = request.Email,
                Subject = $"Ordem {request.OrderId} processada",
                Body = string.Format(ORDER_MAIL_TEMPLATE, request.OrderId, request.OrderDate.ToString("dd/MM/yyyy HH:mm"), url)
            };
            _mailService.Send(mailRequest);
        }

        public async Task Create(OrderServiceDto orderService)
        {
            //Validando os dados
            var userRequest = await _userService.FindById(orderService.UserRequestId);
            if (userRequest == null)
                throw new ApiException("O usuário que solicitou os produtos não existe!");
            var userSurvey = await _repoUserSurvey.FindById(orderService.UserSurveyId);
            if (userSurvey == null)
                throw new ApiException("O ID da área do usuário não existe!");
            var cities = await _repoCity.FindByArea(userSurvey.Geometry);
            if (!cities.Any())
                throw new ApiException("A área informada pelo usuário não pertence a uma cidade coberta pelo serviço!");
            if (cities.Count() != 1)
                throw new ApiException("A área informada pelo usuário pertence a mais de uma cidade!");
            var survey = await _repoSurvey.FindById(orderService.SurveyId);
            if (survey == null)
                throw new ApiException("O levantamento solicitado não existe!");
            if (!survey.Geometry.Contains(userSurvey.Geometry))
                throw new ApiException("A área informada não está contida no levantamento!");

            long? idVoucher = null;
            if (!await _userService.IsAdmin(_userService.CurrentUserId))
            {

                if ((orderService.VoucherCode == null) || (string.IsNullOrEmpty(orderService.VoucherCode.Trim())))
                    throw new ApiException("O código do voucher não foi informado!");

                //Validando o voucher
                idVoucher = await _voucherService.VoucherIsValid(orderService.VoucherCode, userSurvey.Area);
            }

            //Criando a ordem
            // TODO: Criar serviço para calcular o preço
            var newOrder = new Order
            {
                OrderId = Guid.NewGuid().ToString(),
                Date = DateTime.Now,
                Price = 0/*Math.Round(userSurvey.Area * 0.50, 2)*/,
                UserId = orderService.UserRequestId,
                Status = OrderStatusEnum.Created,
                UserSurveyId = orderService.UserSurveyId,
                SurveyId = orderService.SurveyId,
                VoucherId = idVoucher
            };
            await _repoOrder.Create(newOrder);

            if (idVoucher.HasValue)
                await _voucherService.UseVoucher(idVoucher.Value);

            var requestGeometry = await _ntsService.Transform(userSurvey.Geometry, survey.Srid);

            var backgroundRequest = new SurveyServiceBackgroundRequestDto
            {
                OrderId = newOrder.OrderId,
                OrderDate = newOrder.Date,
                Wkt = requestGeometry.AsText(), //evitar serializar geometria (por isso passo o wkt e não a geometria)
                Srid = requestGeometry.SRID,
                Folder = survey.Folder,
                Email = userRequest.Email
            };

            try
            {
                BackgroundJob.Enqueue(() => this.RequestProducts(backgroundRequest));
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message, exc);
                throw;
            }
        }
    }
}
