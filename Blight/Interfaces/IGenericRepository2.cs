using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Interfaces
{
    public interface IGenericRepository2<T> where T : class
    {
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> predicate = null);
        Task<T> GetById(int id);
        Task<T> Create(IDto entity);
        Task Delete(int id);
        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns>T1 - Dto mapped to T - generic object. T2 - bool - isUpdated. Update system depends on developer</returns>
        Task<T> Update(int id, IDto dto);
        Task<T> FindElement(Expression<Func<T, bool>> predicate);



    }
}
