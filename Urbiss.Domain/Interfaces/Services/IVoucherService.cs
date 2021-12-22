using System.Collections.Generic;
using System.Threading.Tasks;
using Urbiss.Domain.Dtos;

namespace Urbiss.Domain.Interfaces
{
    public interface IVoucherService
    {
        Task Create(VoucherCreateDto voucher);
        Task SendVouchersMail(VoucherSendMail voucherSendMail);
        Task<long> VoucherIsValid(string voucherCode, double area);
        Task UseVoucher(long id);
    }
}
