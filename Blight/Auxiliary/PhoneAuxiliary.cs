using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Blight.Entieties;
using Blight.Infrastructure;
using Blight.Interfaces;
using Blight.Models;
using Microsoft.EntityFrameworkCore;

namespace Blight.Auxiliary
{
    public class PhoneAuxiliary:IAuxiliary<PhoneNumber>
    {
        private readonly BlightDbContext _blightDbContext;


        public PhoneAuxiliary(BlightDbContext blightDbContext)
        {
            _blightDbContext = blightDbContext;
        }

        public async Task<bool> UpdateIfExist(PhoneNumber phoneNumber)
        {
            var existingPhoneNumber = await IsExistingInDb(phoneNumber);

            if(existingPhoneNumber is null)
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

        public async Task<bool> TryAddToDb(PhoneNumber phoneNumber)
        {
            var result = _blightDbContext.PhoneNumbers
                .Add(phoneNumber);

            if(result.State == EntityState.Added)
            {
                await _blightDbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> FindById(int id)
        {
            var locatedNumber = await _blightDbContext.PhoneNumbers
                .SingleOrDefaultAsync(x => x.Id == id);

            if (locatedNumber is null)
            {
                return false;
            }

            return true;

        }

        private async Task<PhoneNumber> IsExistingInDb(PhoneNumber phoneNumber)
        {
            var existingPhoneNumber = await _blightDbContext.PhoneNumbers
                .SingleOrDefaultAsync(t => t.Prefix == phoneNumber.Prefix && t.Number == phoneNumber.Number || t.Id==phoneNumber.Id);

            if (existingPhoneNumber is null)
            {
                return null;
            }

            return existingPhoneNumber;
        }



    }
}
