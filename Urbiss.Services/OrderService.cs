using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Exceptions;
using Urbiss.Domain.Interfaces;
using Urbiss.Domain.Models;

namespace Urbiss.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repoOrder;
        private readonly IOrderRequestProductBackgroundService _orderRequestBackgroundService;
        private readonly AppSettingsDto _appSettings;

        public OrderService(IOrderRepository repoOrder, IOrderRequestProductBackgroundService orderRequestBackgroundService, IOptions<AppSettingsDto> appSettings)
        {
            this._repoOrder = repoOrder;
            this._orderRequestBackgroundService = orderRequestBackgroundService;
            this._appSettings = appSettings.Value;
        }

        public async Task Create(OrderServiceDto orderService)
        {
            await _orderRequestBackgroundService.Create(orderService);
        }

        public async Task<IEnumerable<OrderListDto>> List(long userId)
        {
            return await _repoOrder.FindAllToList(userId);
        }

        public async Task<string> GetFilePathToDownload(string orderId, int userId)
        {
            var order = await _repoOrder.FindByOrderId(orderId);
            if (order == null)
                throw new ApiException($"O pedido {orderId} não foi encontrado!");
            if (order.Status != OrderStatusEnum.Processed)
                throw new ApiException($"O pedido {orderId} ainda não foi processado!");
            if (order.UserId != userId)
                throw new ApiException($"O pedido {orderId} não pertence ao usuário que está solicitando o download!");
            var result = AppSettingsDto.GetAppFolder(_appSettings.OutputFolder, orderId, $"{orderId}.zip");
            return result;
        }
    }
}
