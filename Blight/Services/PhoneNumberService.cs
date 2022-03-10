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
using Blight.Exceptions;

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


        public async Task Delete(int id)
        {

            var phoneNumber = await _auxiliary.FindById(id);

            if (phoneNumber is null)
            {
                throw new NotFoundException("Number not Found");
            }

            var result = _blightDbContext.PhoneNumbers
                .Remove(phoneNumber);

            if (result.State != EntityState.Deleted)
                throw new DataBaseException("Sorry, something went wrong");

            await _blightDbContext.SaveChangesAsync();
        }

        public async Task<PhoneNumber> Get(int id)
        {
            var phoneNumber = await _auxiliary.FindById(id);
            if(phoneNumber is null)
            {
                throw new NotFoundException("Number not Found");
            }

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
            var newPhoneNumber = _mapper.Map<PhoneNumber>(dto);

            var isUpdated = await _auxiliary.UpdateIfExist(newPhoneNumber);

            if(isUpdated)
            {
                return newPhoneNumber;
            }

            newPhoneNumber.Notified++;

            var isAdded = await _auxiliary.TryAddToDb(newPhoneNumber);
            if(!isAdded)
            {
                throw new DataBaseException("Sorry, something went wrong");
            }

            return newPhoneNumber;
        }
    }
}
