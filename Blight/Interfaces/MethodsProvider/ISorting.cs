using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Blight.Interfaces.MethodsProvider
{
    public interface ISorting<T>
    {
        IQueryable<IDto> Sort(IQueryable<IDto> entryQueryable, IPaginationObj paginationObj);
    }
}
