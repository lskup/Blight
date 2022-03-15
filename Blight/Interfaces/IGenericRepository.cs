using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Interfaces
{
    public interface IGenericRepository<T> where T:class
    {
        Task<IEnumerable<T>> GetAll(Func<T,bool> predicate = null);
        Task<T> GetById(int id);
        Task<bool> Add(T entity);
        Task<bool> Delete(int id);
        Task<bool> Update(T entity);
        Task<IEnumerable<T>> Find(Func<T,bool> predicate);



    }
}
