using AspNetCoreHero.Results;
using System.Collections.Generic;
using System.Threading.Tasks;
using Urbiss.Domain.Dtos;

namespace Urbiss.Domain.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderListDto>> List(long userId);
        Task Create(OrderServiceDto order);
        Task<string> GetFilePathToDownload(string orderId, int userId);
    }

    public interface IOrderRequestProductBackgroundService
    {
        Task Create(OrderServiceDto order);
    }
}
