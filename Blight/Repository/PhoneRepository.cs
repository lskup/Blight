using Blight.Entieties;
using Blight.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Blight.Exceptions;
using Blight.Interfaces;
using AutoMapper;

namespace Blight.Repository
{
    public class PhoneRepository : GenericRepository<PhoneNumber>
    {

        public PhoneRepository(BlightDbContext blightDbContext,IMapper mapper) : base(blightDbContext,mapper)
        {

        }
        public override async Task<bool> Update(PhoneNumber phoneNumber)
        {
            var existingPhoneNumber = await FindElement
                (e=>e.Number == phoneNumber.Number &&
                 e.Prefix==phoneNumber.Prefix);

            if (existingPhoneNumber is null)
            {
                return false;
            }

            phoneNumber.Id = existingPhoneNumber.Id;
            phoneNumber.Notified = existingPhoneNumber.Notified;
            phoneNumber.Notified++;

            if (phoneNumber.IsBully == false)
            {
                if (phoneNumber.Notified > 20)
                    phoneNumber.IsBully = true;
            }

            _blightDbContext.Entry(existingPhoneNumber)
                .CurrentValues
                .SetValues(phoneNumber);

            await _blightDbContext.SaveChangesAsync();

            return true;
        }
    }
}
