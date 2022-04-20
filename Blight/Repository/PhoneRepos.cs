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

            var updatedNumber = UpdatePhoneNumberNotification(existingPhoneNumber,MethodType.Create);
        }

        private async Task<PhoneNumber> UpdatePhoneNumberNotification(PhoneNumber existingInDb,MethodType methodType)
        {
            var updateData = new PhoneNumber();

            updateData.Id = existingInDb.Id;
            updateData.Prefix = existingInDb.Prefix;
            updateData.Number = existingInDb.Number;
            updateData.Notified = existingInDb.Notified;

            if (methodType == MethodType.Create)
                updateData.Notified++;

            if(methodType == MethodType.Delete)
                updateData.Notified--;

            if(updateData.Notified>20)
            {
                updateData.IsBully = true;
            }
            else
            {
                updateData.IsBully = false;
            }

            _blightDbContext.Entry(existingInDb)
                .CurrentValues
                .SetValues(updateData);

            await _blightDbContext.SaveChangesAsync();

            return updateData;


            // //////////////////////////////////////////////////



            //if(methodType == MethodType.Create)
            //{
            //    activeUser.BlockedNumbers.Add(existingInDb);
            //}
            //if (methodType == MethodType.Delete)
            //{
            //    activeUser.BlockedNumbers.Remove(existingInDb);
            //}

            //await _blightDbContext.SaveChangesAsync();

            //return updateData;
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
                        var result = UpdateNotification(phoneNumber,)
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
