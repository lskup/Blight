using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Blight.Entieties;
using Blight.Infrastructure;
using Blight.Models;
using Microsoft.EntityFrameworkCore;

namespace Blight.Auxiliary
{
    public class Auxiliary
    {
        private readonly BlightDbContext _blightDbContext;

        public Auxiliary(BlightDbContext blightDbContext)
        {
            _blightDbContext = blightDbContext;
        }

        public async Task<bool> UpdateNumberIfExist(PhoneNumber phoneNumber)
        {

            var existingPhoneNumber = await _blightDbContext.PhoneNumbers
                .SingleOrDefaultAsync(t => t.Prefix == phoneNumber.Prefix && t.Number == phoneNumber.Number);

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





    }
}
