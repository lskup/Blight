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
    public class PhoneRepos2 : GenericRepository2<PhoneNumber>
    {
        private readonly IMapper _mapper;

        public PhoneRepos2(BlightDbContext blightDbContext, IMapper mapper) : base(blightDbContext,mapper)
        {
            _mapper = mapper;
        }
        public override async Task<PhoneNumber> Create(IDto dto)
        {
            var phoneNumber = _mapper.Map<PhoneNumber>(dto);

            var existingPhoneNumber = await FindElement
                (e => e.Number == phoneNumber.Number &&
                 e.Prefix == phoneNumber.Prefix);

            if (existingPhoneNumber is null)
            {
                return await base.Create(dto);
            }

            return await Update(phoneNumber,existingPhoneNumber);
        }

        private async Task<PhoneNumber> Update(PhoneNumber mappedNumber, PhoneNumber existingPhoneNumber)
        {
            mappedNumber.Id = existingPhoneNumber.Id;
            mappedNumber.Notified = existingPhoneNumber.Notified;
            mappedNumber.Notified++;

            if (mappedNumber.IsBully == false)
            {
                if (mappedNumber.Notified > 20)
                    mappedNumber.IsBully = true;
            }

            _blightDbContext.Entry(existingPhoneNumber)
                .CurrentValues
                .SetValues(mappedNumber);

            await _blightDbContext.SaveChangesAsync();

            return mappedNumber;
        }
    }
}
