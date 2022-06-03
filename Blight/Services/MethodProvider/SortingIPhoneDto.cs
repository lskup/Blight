using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Entieties;
using Blight.Enums;
using Blight.Interfaces;
using Blight.Interfaces.MethodsProvider;

namespace Blight.Services.MethodProvider
{
    public class SortingIPhoneDto : ISorting<PhoneNumber>
    {
        public IQueryable<IDto> Sort(IQueryable<IDto> entryQueryable, IPaginationObj paginationObj)
        {
            var phonePagination = paginationObj as IPhonePagination;
            var phoneQueryable = entryQueryable as IQueryable<IPhoneNumberDto>;

            phoneQueryable = phonePagination.sortDirection == SortDirection.Asc ?
                            phoneQueryable.OrderBy(x => x.Notified) :
                            phoneQueryable.OrderByDescending(x => x.Notified);

            return phoneQueryable;
        }
    }
}
