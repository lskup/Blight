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
        private readonly IAuxiliary<PhoneNumber> _auxiliary;

        public PhoneNumberService(BlightDbContext blightDbContext, IMapper mapper, IAuxiliary<PhoneNumber> auxiliary)
        {
            _blightDbContext = blightDbContext;
            _mapper = mapper;
            _auxiliary = auxiliary;
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

            var isUpdated = await _auxiliary.UpdateIfExist(newPhoneNumber);

            if(isUpdated)
            {
                return newPhoneNumber;
            }

            newPhoneNumber.Notified++;

            var isAdded = await _auxiliary.TryAddToDb(newPhoneNumber);
            if(isAdded)
            {
                return newPhoneNumber;

            }
            return null;

        }

        public async Task<bool> Put(int id, PhoneNumberDto dto)
        {
            var newPhoneNumber = _mapper.Map<PhoneNumber>(dto);
            newPhoneNumber.Id = id;

            var isExistingInDb = await _auxiliary.FindById(id);

            if(!isExistingInDb)
            {
                return false;
            }

            var isUpdated = await _auxiliary.UpdateIfExist(newPhoneNumber);

            if(!isUpdated)
            {
                return false;
            }

            return true;
        }
    }
}
