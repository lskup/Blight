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
        /// <summary>
        /// The method receives nullable Id and object from form as some Dto. The method purpose is
        /// to map dto to generic object and
        /// check if exist in Database.
        /// 1.Object is not exist in Db - 
        /// 
        /// T is the mapped object from dto. Bool determines if object was addidionally updated
        /// or not. The update system is set by developer. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns>T1 - Dto mapped to T - generic object. T2 - bool - isUpdated</returns>
        Task<Tuple<T,bool>> CreateOrUpdate(int? id,IDto dto);
        Task<T> FindElement(Expression<Func<T, bool>> predicate);



    }
}
