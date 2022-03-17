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
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> predicate = null);
        Task<T> GetById(int id);
        Task<T> Create(IDto entity);
        Task Delete(int id);
        Task<bool> Update(T entity);
        Task<T> FindElement(Expression<Func<T, bool>> predicate);



    }
}
