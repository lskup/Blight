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
using Blight.Models;

namespace Blight.Repository
{
    public class PhoneRepos : GenericRepository<PhoneNumber>,IPhoneRepository
    {
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public PhoneRepos(BlightDbContext blightDbContext, IMapper mapper, IUserContextService userContextService) : base(blightDbContext, mapper)
        {
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async override Task<IEnumerable<IDto>> GetAll(Expression<Func<PhoneNumber, bool>>? predicate)
        {
            List<PhoneNumber> listOfNumbers;

            if (predicate is null)
            {
                listOfNumbers = await _dbSet
                    .Include(x=>x.Users)
                    .AsNoTracking()
                    .ToListAsync();

            }
            else
            {
                listOfNumbers = await _dbSet
                    .Include(x => x.Users)
                    .Where(predicate)
                    .AsNoTracking()
                    .ToListAsync();

            }

            var mappedList = _mapper.Map<IEnumerable<PhoneNumberViewModel>>(listOfNumbers);
            return mappedList;

        }
        public async Task<IEnumerable<IDto>> GetUserBlockedNumbers()
        {
            var activeUser = await _blightDbContext
                    .Users
                    .Include(n => n.BlockedNumbers)
                    .SingleOrDefaultAsync(x => x.Id == _userContextService.GetUserId);

            var blockedNumbers = activeUser.BlockedNumbers;
            var mappedNumbers = _mapper.Map<List<PhoneNumberViewModel>>(blockedNumbers);

            return mappedNumbers;
        }

        public async override Task<PhoneNumber> FindElement(Expression<Func<PhoneNumber, bool>> predicate)
        {
            var result = await _dbSet.FirstOrDefaultAsync(predicate);

            return result;
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

                userNumberList.Add(phoneNumber);
                await _blightDbContext.SaveChangesAsync();
                phoneNumber.Users = null;

                return phoneNumber;

        }

        public async override Task Delete(int id)
        {
            var phoneNumber = await _dbSet.FirstOrDefaultAsync(i =>i.Id == id) as PhoneNumber;
            if(phoneNumber is null)
            {
                throw new NotFoundException("Number not found");
            }

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
                    }
                }
            }
            if(userRole ==2)
            {
                _dbSet.Remove(phoneNumber);
            }

        }

        public async override Task<IDto> GetById(int id)
        {
            var element = await _dbSet
                .Include(x=>x.Users)
                .FirstOrDefaultAsync(i=>i.Id == id);

            if (element is null)
            {
                throw new NotFoundException("Number not found");
            }

            var result = _mapper.Map<PhoneNumberViewModel>(element);

            return result;
        }


        private enum MethodType
        {
            Create,
            Delete
        }


    }
}
