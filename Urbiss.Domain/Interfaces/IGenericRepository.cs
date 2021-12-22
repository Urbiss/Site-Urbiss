using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Urbiss.Domain.Models;

namespace Urbiss.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        DbContext Context { get; }
        Task<T> Create(T entity);
        Task<T> Update(T entity);
        Task Delete(long id);
        Task<T> FindById(long id);
        Task<bool> Exists(long id);
        Task<List<T>> FindAll();
    }
}
