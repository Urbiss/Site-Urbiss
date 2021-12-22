using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Urbiss.Domain.Interfaces;
using Urbiss.Domain.Models;

namespace Urbiss.Repository
{
    public class VoucherRepository : GenericRepository<Voucher>, IVoucherRepository
    {
        public VoucherRepository(UrbissDbContext context) : base(context)
        {
        }

        public async Task<Voucher> FindByCode(string code)
        {
            return await _dataset.Where(v => v.Code.ToUpper().Equals(code.ToUpper())).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Voucher>> ListValidsByEmail(string email)
        {
            email = email.ToLower();
            return await _dataset.Where(v => ((v.Email.ToLower().Equals(email.ToLower())) && (v.Expiration >= DateTime.Now.Date) && (v.Status == VoucherStatusEnum.Pending))).ToListAsync();
        }
    }
}
