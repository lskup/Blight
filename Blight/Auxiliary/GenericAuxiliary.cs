using Blight.Infrastructure;
using Blight.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Blight.Auxiliary
{
    public class GenericAuxiliary<T> : IAuxiliary<T> where T:class
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

        public Task<bool> TryAddToDb(T genericObject)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateIfExist(T genericObject)
        {
            throw new NotImplementedException();
        }
    }
}
