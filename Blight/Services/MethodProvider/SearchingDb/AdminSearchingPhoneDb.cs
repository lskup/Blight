using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Interfaces;
using Blight.Interfaces.MethodsProvider;
using Microsoft.EntityFrameworkCore;
using Blight.Entieties;
using Blight.Models;

namespace Blight.Services.MethodProvider.SearchingDb
{
    public class AdminSearchingPhoneDb : ISearchingPhoneDbSet
    {
        public IQueryable<IPhoneNumberDto> SearchPhoneDbWithCriteria(DbSet<PhoneNumber> phoneDbSet, IPaginationObj paginationObj)
        {
            var phonePagination = paginationObj as IPhonePagination;

            var entryList = phoneDbSet
                .AsNoTracking()
                .Select(x => new AdminPhoneNumberViewModel
                {
                    Id = x.Id,
                    Prefix = x.Prefix,
                    Number = x.Number,
                    Notified = x.Users.Count(),
                    IsBullyTreshold = x.IsBullyTreshold,
                })
                .Where(r => phonePagination.onlyBullyNumbers == false || r.Notified >= r.IsBullyTreshold)
                .Where(r => paginationObj.SearchPhrase == null ||
                        r.Number.Contains(paginationObj.SearchPhrase) ||
                        r.Prefix.Contains(paginationObj.SearchPhrase));

            return entryList;
        }



    }
}
