using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Interfaces.MethodsProvider
{
    public interface IPaginating
    {
        IPagedResult<IDto> Paginate(IQueryable<IDto> entryQueryable, IPaginationObj paginationQuery);
    }
}
