using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Interfaces;
using Blight.Interfaces.MethodsProvider;
using Blight.Models;

namespace Blight.Services.MethodProvider
{
    public class PaginatingMethod : IPaginating
    {
        public IPagedResult<IDto> Paginate(IQueryable<IDto> entryQueryable, IPaginationObj paginationQuery)
        {
            var paginatedList = entryQueryable
                .Skip(paginationQuery.PageSize * (paginationQuery.PageNumber - 1))
                .Take(paginationQuery.PageSize)
                .ToList();

            var recordsTotal = entryQueryable.Count();

            var pageResult =
                new PagedResult<IDto>(paginatedList, recordsTotal, paginationQuery.PageSize, paginationQuery.PageNumber);

            return pageResult;
        }
    }
}
