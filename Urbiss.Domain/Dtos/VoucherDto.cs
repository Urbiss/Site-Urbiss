using System;
using Urbiss.Domain.Models;

namespace Urbiss.Domain.Dtos
{
    public class VoucherCreateDto
    {
        public int Quantity { get; set; }
        public double Area { get; set; }
        public string Email { get; set; }
        public int Days { get; set; }
    }

    public class VoucherSendMail
    {
        public string Email { get; set; }
    }
}
