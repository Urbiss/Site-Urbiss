using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Exceptions;
using Urbiss.Domain.Interfaces;
using Urbiss.Domain.Models;

namespace Urbiss.Repository
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(UrbissDbContext context) : base(context)
        {
        }

        public async Task<Order> FindByOrderId(string orderId)
        {
            return await _dataset.Where(c => c.OrderId.Equals(orderId)).FirstOrDefaultAsync();
        }

        public override Task<List<Order>> FindAll()
        {
            throw new ApiException("Utilizar o método FindAllToList");
        }

        public async Task<IEnumerable<OrderListDto>> FindAllToList(long userId)
        {
            return await _context.Orders
                .Include(o => o.UserSurvey)
                .Where(o => o.UserId == userId)
                .Select(o => new OrderListDto
                {
                    Id = o.Id,
                    OrderId = o.OrderId,
                    Date = o.Date,
                    Area = o.UserSurvey.Area,
                    Price = o.Price,
                    Status = o.Status                    
                }).ToListAsync();               
        }
    }
}
