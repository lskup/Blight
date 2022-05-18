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
using Blight.Enums;

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

        public async override Task<IPagedResult<IDto>> GetAllPaginated(IPagination paginationPhoneQuery)
        {
            var paginationPhoneObj = paginationPhoneQuery as PaginationPhoneQuery;
            List<PhoneNumber> finalList;
            //List<PhoneNumber> entryList;

            var entryList = await _dbSet
                .Include(x => x.Users)
                .AsNoTracking()
                .Where(r => paginationPhoneObj.SearchPhrase == null ||
                                        (r.Number.Contains(paginationPhoneObj.SearchPhrase)) || 
                                        (r.Prefix.Contains(paginationPhoneObj.SearchPhrase)))
                .ToListAsync();

            switch (paginationPhoneObj.sortDirection)
            {
                case SortDirection.Asc:
                    {
                        entryList = entryList.OrderBy(x => x.Notified)
                                             .ToList();
                        break;
                    }
                case SortDirection.Desc:
                    {
                        entryList = entryList.OrderByDescending(x => x.Notified)
                                             .ToList();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            if (paginationPhoneObj.onlyBullyNumbers ==true)
            {
                finalList = entryList.Where(x => x.IsBully == true)
                                     .ToList();
            }
            else
            {
                finalList = entryList;
            }

            var paginatedList = finalList
                .Skip(paginationPhoneObj.PageSize * (paginationPhoneObj.PageNumber - 1))
                .Take(paginationPhoneObj.PageSize)
                .ToList();

            IEnumerable<IDto> mappedList;

            if (activeUser.RoleId==2)
            {
                mappedList = _mapper.Map<IEnumerable<AdminPhoneNumberViewModel>>(paginatedList);
            }

            else
            {
                mappedList = _mapper.Map<IEnumerable<PhoneNumberViewModel>>(paginatedList);
            }

            var recordsTotal = finalList.Count();

            var pageResult =
                new PagedResult<IDto>(mappedList, recordsTotal, paginationPhoneObj.PageSize, paginationPhoneObj.PageNumber);

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

            var recordsTotal = paginatedList.Count();

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
                await _dbSet.AddAsync(phoneNumber);
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
                var phoneNumberinUserBlockedList = activeUser
                                .BlockedNumbers
                                .FirstOrDefault(x => x.Prefix == phoneNumber.Prefix && x.Number == phoneNumber.Number);
                if(phoneNumberinUserBlockedList == null)
                {
                    throw new ForbiddenException("Number not belong to your list");
                }
                activeUser.BlockedNumbers.Remove(phoneNumberinUserBlockedList);

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

        public async Task<string> SetIsBullyTreshold(int id, int treshold)
        {
            var phoneNumber = await _dbSet
                    .FirstOrDefaultAsync(i => i.Id == id);

            if (phoneNumber is null)
            {
                throw new NotFoundException("Number not found");
            }

            phoneNumber.IsBullyTreshold = treshold;

            _blightDbContext.Entry(phoneNumber)
                            .CurrentValues
                            .SetValues(phoneNumber);

            await _blightDbContext.SaveChangesAsync();

            return $"IsBullyTreshold for {phoneNumber.Prefix}{phoneNumber.Number} set to {treshold}";
        }

        public async Task<IEnumerable<IDto>> SyncBlockedNumbers(IEnumerable<IDto> userBlockedNumbers)
        {
            List<PhoneNumberDto> dtosList = userBlockedNumbers as List<PhoneNumberDto>;

            activeUser.BlockedNumbers.Clear();

            _blightDbContext.Entry(activeUser)
                            .CurrentValues
                            .SetValues(activeUser);

            await _blightDbContext.SaveChangesAsync();

            foreach (var item in dtosList)
            {
                await Create(item);
            }

            return await GetAllBullyNumbersDto();
        }

        public async Task<IEnumerable<IDto>> GetAllBullyNumbersDto()
        {
            var allBlockedNumbers = await _dbSet
                    .Include(o => o.Users)
                    .Where(x=>x.Users.Count >= x.IsBullyTreshold)
                    .ToListAsync();

            var dtos = _mapper.Map<List<PhoneNumberDto>>(allBlockedNumbers);

            return dtos;
        }
    }
}
