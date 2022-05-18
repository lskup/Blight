using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Interfaces
{
    public interface IGenericRepository<T> where T : class,IDto
    {
        Task<IPagedResult<IDto>> GetAllPaginated(IPagination paginationQuery);
        Task<IDto> GetById(int id);
        Task<T> Create(IDto entity);
        Task Delete(int id);
        Task<T> Update(int id, IDto dto);
        Task<T> FindElement(Expression<Func<T, bool>> predicate);



    }
}
