using Blight.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Blight.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Blight.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T:class
    {
        protected readonly BlightDbContext _blightDbContext;
        internal DbSet<T> _dbSet;
        protected readonly ILogger _logger;

        public GenericRepository(BlightDbContext blightDbContext, DbSet<T> dbSet, ILogger logger)
        {
            _blightDbContext = blightDbContext;
            _dbSet = blightDbContext.Set<T>();
            _logger = logger;
        }

        public virtual async Task<bool> Add(T entity)
        {
            await _dbSet.AddAsync(entity);
            return true;
        }

        public virtual Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<IEnumerable<T>> Find(Func<T, bool> predicate)
        {
            var result = await _dbSet.Where(predicate)
                .AsQueryable()
                .ToListAsync();

            return result;
        }
                
        
        public virtual async Task<IEnumerable<T>> GetAll(Func<T,bool> predicate = null)
        {
            var result = await _dbSet.Where(predicate)
                .AsQueryable()
                .AsNoTracking()
                .ToListAsync();

            return result;


        }

        public virtual async Task<T> GetById(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual Task<bool> Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
