using Blight.Entieties;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Blight.Interfaces.MethodsProvider
{
    public interface ISearchingUserDbSet
    {
        IQueryable<IDto> SearchUserDbWithCriteria(DbSet<User> dbSet, IPaginationObj paginationObj);
    }
}
