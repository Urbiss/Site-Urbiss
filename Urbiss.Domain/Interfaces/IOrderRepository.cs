using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Threading.Tasks;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Models;

namespace Urbiss.Domain.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Order> FindByOrderId(string orderId);
        Task<IEnumerable<OrderListDto>> FindAllToList(long userId);
    }
}
