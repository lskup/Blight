using AutoMapper;
using Blight.Entieties;
using Blight.Infrastructure;
using Blight.Interfaces;
using Blight.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Services
{
    public class PhoneNumberService : IPhoneNumberService
    {

        private readonly BlightDbContext _blightDbContext;
        private readonly IMapper _mapper;

        public PhoneNumberService(BlightDbContext blightDbContext, IMapper mapper)
        {
            _blightDbContext = blightDbContext;
            _mapper = mapper;
        }


        public async Task<bool> Delete(int id)
        {
            var number = await _blightDbContext.PhoneNumbers
                .SingleOrDefaultAsync(x => x.Id == id);

            if (number is null)
            {
                return false;
            }

            var result = _blightDbContext.PhoneNumbers
                .Remove(number);

            await _blightDbContext.SaveChangesAsync();


            if (result.State == EntityState.Deleted)
                return true;
            else
                return false;
        }

        public async Task<PhoneNumber> Get(int id)
        {
            var phoneNumber = await _blightDbContext.PhoneNumbers
                .SingleOrDefaultAsync(x => x.Id == id);

            /*
            var phoneNumberViewModel = _mapper.Map<PhoneNumberDto>(phoneNumber);

            if (phoneNumberViewModel is null)
            {
                return null;
            }
            */

            return phoneNumber;
        }

        public async Task<ICollection<PhoneNumber>> GetAll(bool onlyBullyNumbers)
        {
            if(!onlyBullyNumbers)
            {
                var phoneNumbers = await _blightDbContext.PhoneNumbers
                .ToListAsync();

                return phoneNumbers;
            }

            var bullyPhoneNumbers = await _blightDbContext.PhoneNumbers
                .Where(x => x.IsBully == true)
                .ToListAsync();
            
                

            return bullyPhoneNumbers;

        }

        public async Task<PhoneNumber> Post(PhoneNumberDto dto)
        {
            if(dto==null)
            {
                return null;
            }

            var newPhoneNumber = _mapper.Map<PhoneNumber>(dto);

            var existingPhoneNumber = await _blightDbContext.PhoneNumbers
                .SingleOrDefaultAsync(t => t.Prefix == dto.Prefix && t.Number == dto.Number);

            if(existingPhoneNumber != null)
            {
                newPhoneNumber.Id = existingPhoneNumber.Id;
                newPhoneNumber.Notified = existingPhoneNumber.Notified;
                newPhoneNumber.Notified++;

                if(newPhoneNumber.IsBully == false)
                {
                    if (newPhoneNumber.Notified > 20)
                        newPhoneNumber.IsBully = true;
                }

                _blightDbContext.Entry(existingPhoneNumber)
                    .CurrentValues
                    .SetValues(newPhoneNumber);

                await _blightDbContext.SaveChangesAsync();
                return newPhoneNumber;
            }

            newPhoneNumber.Notified++;

            var result = _blightDbContext.PhoneNumbers
                .Add(newPhoneNumber);

            await _blightDbContext.SaveChangesAsync();

            return result.Entity;
        }

        public async Task<bool> Put(int id, PhoneNumberDto dto)
        {
            var updatedNumber = _mapper.Map<PhoneNumber>(dto);

            var locatedNumber = await _blightDbContext.PhoneNumbers
                .SingleOrDefaultAsync(x => x.Id == id);

            if (locatedNumber is null)
            {
                return false;
            }

            var existingNumber = await _blightDbContext.PhoneNumbers
                .SingleOrDefaultAsync(k => k.Prefix == dto.Prefix && k.Number == dto.Number);
                
            if(existingNumber != null)
            {
                return false;
            }

            updatedNumber.Id = id;
            updatedNumber.Notified = locatedNumber.Notified;
            

            _blightDbContext.Entry(locatedNumber).CurrentValues
                .SetValues(updatedNumber);

            await _blightDbContext.SaveChangesAsync();

            return true;
        }
    }
}
