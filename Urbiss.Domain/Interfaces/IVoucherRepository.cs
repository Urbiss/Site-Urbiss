using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Threading.Tasks;
using Urbiss.Domain.Dtos;
using Urbiss.Domain.Models;

namespace Urbiss.Domain.Interfaces
{
    public interface IVoucherRepository : IGenericRepository<Voucher>
    {
        Task<Voucher> FindByCode(string code);
        Task<IEnumerable<Voucher>> ListValidsByEmail(string email);
    }
}
