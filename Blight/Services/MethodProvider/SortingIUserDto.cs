using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Blight.Entieties;
using Blight.Enums;
using Blight.Interfaces;
using Blight.Interfaces.MethodsProvider;

namespace Blight.Services.MethodProvider
{
    public class SortingIUserDto : ISorting<User>
    {
        public IQueryable<IDto> Sort(IQueryable<IDto> entryQueryable, IPaginationObj paginationObj)
        {
            var userPagination = paginationObj as IUserPagination;
            var userQueryable = entryQueryable as IQueryable<IUserDto>;

            var userSortOptions_userProperties_Pairs = new Dictionary<SortUserBy, Expression<Func<IUserDto, object>>>
            {
                {SortUserBy.FirstName,p=>p.FirstName},
                {SortUserBy.LastName,p=>p.LastName},
                {SortUserBy.Nationality,p=>p.Nationality},
                {SortUserBy.RoleId,p=>p.RoleId},
            };

            var selected = userSortOptions_userProperties_Pairs[userPagination.sortUserBy];

            userQueryable = userPagination.sortDirection == SortDirection.Asc
                                            ? userQueryable.OrderBy(selected)
                                            : userQueryable.OrderByDescending(selected);

            return userQueryable;
        }
    }
}
