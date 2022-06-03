using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Entieties;
using Blight.Interfaces;
using Blight.Interfaces.MethodsProvider;
using Blight.Models;
using Microsoft.EntityFrameworkCore;

namespace Blight.Services.MethodProvider.SearchingDb.Shared
{
    public class SearchingUserDb : ISearchingUserDbSet
    {
        public IQueryable<IDto> SearchUserDbWithCriteria(DbSet<User> dbSet, IPaginationObj paginationQuery)
        {
            var paginationObj = paginationQuery as PaginationUserQuery;

            var entryList = dbSet
                .AsNoTracking()
                .Where(r => paginationObj.onlyBannedUsers == false || r.Banned == true)
                .Where(r => paginationObj.SearchPhrase == null ||
                    r.FirstName.ToLower().Contains(paginationObj.SearchPhrase.ToLower()) ||
                    r.LastName.ToLower().Contains(paginationObj.SearchPhrase.ToLower()) ||
                    r.Nationality.ToLower().Contains(paginationObj.SearchPhrase.ToLower()));

            return entryList;
        }
    }
}
