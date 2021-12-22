using System;
using Urbiss.Domain.Models;

namespace Urbiss.Domain.Dtos
{
    public class OrderDto
    {
        public int SurveyId { get; set; }
        public int UserSurveyId { get; set; }
        public string VoucherCode { get; set; }
    }

    public class OrderServiceDto : OrderDto
    {
        public int UserRequestId { get; set; }
    }

    public class OrderListDto
    {
        public long Id { get; set; }
        public string OrderId { get; set; }
        public DateTime Date { get; set; }
        public OrderStatusEnum Status { get; set; }
        public double Area { get; set; }
        public double Price { get; set; }
    }
}
