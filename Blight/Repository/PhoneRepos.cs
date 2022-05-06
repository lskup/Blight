﻿using Blight.Entieties;
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
        private User? findActiveUser => _blightDbContext
                    .Users
                    .Include(n => n.BlockedNumbers)
                    .Include(n => n.Role)
                    .SingleOrDefault(x => x.Id == _userContextService.GetUserId);

        private User? activeUser => findActiveUser is null ?
            throw new ForbiddenException("You have not authority for this action") :
            findActiveUser;

        public async override Task<IPagedResult<IDto>> GetAll(IPagination paginationPhoneQuery)
        {
            var paginationPhoneObj = paginationPhoneQuery as PaginationPhoneQuery;
            List<PhoneNumber> listOfNumbers;

            listOfNumbers = await _dbSet
                .Include(x => x.Users)
                .AsNoTracking()
                .Where(x=>paginationPhoneObj.onlyBlockedNumbers ==false || paginationPhoneObj.onlyBlockedNumbers == true && x.IsBully == true)
                .Where(r => paginationPhoneObj.SearchPhrase == null || (r.Number.Contains(paginationPhoneObj.SearchPhrase)) ||
                        paginationPhoneObj.onlyBlockedNumbers)
                .ToListAsync();

            var paginatedList = listOfNumbers
                .Skip(paginationPhoneObj.PageSize * (paginationPhoneObj.PageNumber - 1))
                .Take(paginationPhoneObj.PageSize)
                .ToList();

            IEnumerable<IDto> result;

            if (activeUser.RoleId==2)
            {
                result = _mapper.Map<IEnumerable<AdminPhoneNumberViewModel>>(paginatedList);
            }

            else
            {
                result = _mapper.Map<IEnumerable<PhoneNumberViewModel>>(paginatedList);
            }

            var recordsTotal = result.Count();

            var pageResult =
                new PagedResult<IDto>(result, recordsTotal, paginationPhoneObj.PageSize, paginationPhoneObj.PageNumber);

            return pageResult;
        }
        public async Task<IPagedResult<IDto>> GetUserAllBlockedNumbers(IPagination paginationQuery)
        {
            var paginationObj = paginationQuery as PaginationQuery;
            var blockedNumbers = activeUser.BlockedNumbers;

            var paginatedList = blockedNumbers
                .Skip(paginationObj.PageSize * (paginationObj.PageNumber - 1))
                .Take(paginationObj.PageSize)
                .ToList();

            var result = _mapper.Map<List<PhoneNumberViewModel>>(blockedNumbers);

            var recordsTotal = result.Count();

            var pageResult =
                new PagedResult<IDto>(result, recordsTotal, paginationObj.PageSize, paginationObj.PageNumber);

            return pageResult;
        }

        public async override Task<PhoneNumber> FindElement(Expression<Func<PhoneNumber, bool>> predicate)
        {
            var result = await _dbSet
                    .Include(x => x.Users)
                    .FirstOrDefaultAsync(predicate);

            return result;
        }

        public override async Task<PhoneNumber> Create(IDto dto)
        {
            if(activeUser.Banned == true)
            {
                throw new ForbiddenException("You are banned, contact with administration");
            }

            var phoneNumber = _mapper.Map<PhoneNumber>(dto);

            var existingPhoneNumber = await FindElement
                (e => e.Number == phoneNumber.Number &&
                 e.Prefix == phoneNumber.Prefix);

            var userNumberList = activeUser.BlockedNumbers;

            foreach (var userNumber in userNumberList)
            {
                if(userNumber.Number == phoneNumber.Number)
                {
                    throw new ForbiddenException("You already block this number");
                }
            }

            if(existingPhoneNumber is null)
            {
                phoneNumber.Users.Add(activeUser);
            }
            else
            {
                existingPhoneNumber.Users.Add(activeUser);
            }

            await _blightDbContext.SaveChangesAsync();

            phoneNumber.Users = new List<User>();
            
            return phoneNumber;

        }

        public async override Task Delete(int id)
        {
            var phoneNumber = await _dbSet.FirstOrDefaultAsync(i =>i.Id == id) as PhoneNumber;
            if(phoneNumber is null)
            {
                throw new NotFoundException("Number not found");
            }

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
            await _blightDbContext.SaveChangesAsync();
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

            if(activeUser.RoleId ==2)
            {
                return _mapper.Map<AdminPhoneNumberViewModel>(element); ;
            }
            var result = _mapper.Map<PhoneNumberViewModel>(element);

            return result;
        }
    }
}
