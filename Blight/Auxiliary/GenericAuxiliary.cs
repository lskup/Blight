using Blight.Infrastructure;
using Blight.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Blight.Entieties;

namespace Blight.Auxiliary
{
    public class GenericAuxiliary<T> : IAuxiliary<T> where T : class
    {
        private readonly BlightDbContext _blightDbContext;
        public GenericAuxiliary(BlightDbContext blightDbContext)
        {
            _blightDbContext = blightDbContext;
        }

        public async Task<T> FindById(int id)
        {
            var entity = await _blightDbContext.FindAsync<T>(id);

            return entity;
        }

        public async Task<bool> TryAddToDb(T genericObject)
        {
            var result = await _blightDbContext.AddAsync<T>(genericObject);

            if (result.State == EntityState.Added)
            {
                await _blightDbContext.SaveChangesAsync();
                return true;
            }

            return false;

        }

        public Task<bool> UpdateIfExist(T genericObject)
        {
            throw new NotImplementedException();
        }
    }

}
