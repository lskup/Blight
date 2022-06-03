using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Interfaces;
using Blight.Entieties;

namespace Blight.Interfaces.MethodsProvider
{
    public interface ISearchingPhoneDbSet
    {
        IQueryable<IPhoneNumberDto> SearchPhoneDbWithCriteria(DbSet<PhoneNumber> dbSet, IPaginationObj paginationObj);

    }
}
