using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Interfaces;
using Blight.Interfaces.MethodsProvider;
using Blight.Entieties;
using Microsoft.EntityFrameworkCore;
using Blight.Models;

namespace Blight.Services.MethodProvider.SearchingDb
{
    public class UserSearchingPhoneDb : ISearchingPhoneDbSet
    {
        public IQueryable<IPhoneNumberDto> SearchPhoneDbWithCriteria(DbSet<PhoneNumber> phoneDbSet, IPaginationObj paginationObj)
        {
            var paginationPhoneObj = paginationObj as IPhonePagination;

            var entryList = phoneDbSet
                .AsNoTracking()
                .Where(r => paginationPhoneObj.onlyBullyNumbers == false || r.Users.Count() >= r.IsBullyTreshold)
                .Select(x => new PhoneNumberViewModel
                {
                    Prefix = x.Prefix,
                    Number = x.Number,
                    Notified = x.Users.Count(),
                })
                .Where(r => paginationPhoneObj.SearchPhrase == null ||
                        r.Number.Contains(paginationPhoneObj.SearchPhrase) ||
                        r.Prefix.Contains(paginationPhoneObj.SearchPhrase));

            return entryList;
        }
    }
}
