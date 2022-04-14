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
    public class PhoneRepos : GenericRepository<PhoneNumber>
    {
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public PhoneRepos(BlightDbContext blightDbContext, IMapper mapper, IUserContextService userContextService) : base(blightDbContext, mapper)
        {
            _mapper = mapper;
            _userContextService = userContextService;
        }
        public override async Task<PhoneNumber> Create(IDto dto)
        {
            var phoneNumber = _mapper.Map<PhoneNumber>(dto);

            var existingPhoneNumber = await FindElement
                (e => e.Number == phoneNumber.Number &&
                 e.Prefix == phoneNumber.Prefix);

            var activeUser = await _blightDbContext
                    .Users
                    .Include(n=>n.BlockedNumbers)
                    .SingleOrDefaultAsync(x => x.Id == _userContextService.GetUserId);

            var userNumberList = activeUser.BlockedNumbers;

            foreach (var userNumber in userNumberList)
            {
                if(userNumber.Number == phoneNumber.Number)
                {
                    throw new ForbiddenException("You already block this number");
                }
            }

            if (existingPhoneNumber is null)
            {
                userNumberList.Add(phoneNumber);
                await _blightDbContext.SaveChangesAsync();
                phoneNumber.Users = null;
                return phoneNumber;
            }

            return await Update(phoneNumber,existingPhoneNumber,activeUser,MethodType.Create);
        }

        private async Task<PhoneNumber> Update(PhoneNumber mappedNumber, PhoneNumber existingPhoneNumber,User activeUser,MethodType methodType)
        {
            mappedNumber.Id = existingPhoneNumber.Id;
            mappedNumber.Notified = existingPhoneNumber.Notified;

            if(methodType == MethodType.Create)
                mappedNumber.Notified++;

            if(methodType == MethodType.Delete)
                mappedNumber.Notified--;

            if(mappedNumber.Notified>20)
            {
                mappedNumber.IsBully = true;
            }
            else
            {
                mappedNumber.IsBully = false;
            }

            _blightDbContext.Entry(existingPhoneNumber)
                .CurrentValues
                .SetValues(mappedNumber);

            activeUser.BlockedNumbers.Add(existingPhoneNumber);
            await _blightDbContext.SaveChangesAsync();

            return mappedNumber;
        }

        public async override Task Delete(int id)
        {
            var phoneNumber = await GetById(id);

            var activeUser = await _blightDbContext
                    .Users
                    .Include(n => n.BlockedNumbers)
                    .SingleOrDefaultAsync(x => x.Id == _userContextService.GetUserId);

            var userRole = activeUser.RoleId;

            if(userRole == 1)
            {
                var userNumberList = activeUser.BlockedNumbers;

                foreach (var userNumber in userNumberList)
                {
                    if (userNumber.Number == phoneNumber.Number)
                    {
                        activeUser.BlockedNumbers.Remove(phoneNumber);
                       // var result = Update()
                       
                    }
                }

            }

        }

        private enum MethodType
        {
            Create,
            Delete
        }


    }
}
