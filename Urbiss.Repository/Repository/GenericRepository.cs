using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Urbiss.Domain.Constants;
using Urbiss.Domain.Interfaces;
using Urbiss.Domain.Models;

namespace Urbiss.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly UrbissDbContext _context;
        protected readonly DbSet<T> _dataset;
        public GenericRepository(UrbissDbContext context)
        {
            this._context = context;
            this._context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            this._dataset = _context.Set<T>();
        }

        public DbContext Context => _context;

        private string _currentUserName = string.Empty;
        protected string CurrentUserName
        {
            get
            {
                if (string.IsNullOrEmpty(_currentUserName))
                {
                    if (_context.ContextAccessor == null)
                        _currentUserName = GlobalConsts.SERVER_USER;
                    else
                        _currentUserName = _context.ContextAccessor.RequestServices.GetService<IUserService>().CurrentUserName;
                }
                return _currentUserName;
            }
        }
        public async virtual Task<T> FindById(long id)
        {
            return await _dataset.SingleOrDefaultAsync(item => item.Id.Equals(id));
        }

        public async virtual Task<List<T>> FindAll()
        {
            return await _dataset.ToListAsync();
        }

        public virtual async Task<T> Create(T entity)
        {
            try
            {
                entity.Creation = DateTime.Now;
                entity.UserCreation = CurrentUserName;
                _dataset.Add(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual async Task Delete(long id)
        {
            var result = _dataset.SingleOrDefault(item => item.Id.Equals(id));
            if (result != null)
            {
                try
                {
                    _dataset.Remove(result);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public virtual async Task<T> Update(T entity)
        {
            var result = _dataset.SingleOrDefault(item => item.Id.Equals(entity.Id));
            if (result != null)
            {
                try
                {
                    _context.Entry(result).CurrentValues.SetValues(entity);
                    result.Modification = DateTime.Now;
                    result.UserModification = CurrentUserName;
                    _context.Entry(result).State = EntityState.Modified;

                    await _context.SaveChangesAsync();
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
                return null;
        }

        public async virtual Task<bool> Exists(long id)
        {
            return await _dataset.AnyAsync(item => item.Id.Equals(id));
        }
    }   
}
