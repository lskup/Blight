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
using Microsoft.Extensions.Logging;
using Blight.Interfaces.MethodsProvider;
using Blight.Services.MethodProvider.SearchingDb;

namespace Blight.Repository
{
    public class PhoneRepos : GenericRepository<PhoneNumber>,IPhoneRepository
    {
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;
        private readonly ILogger<PhoneRepos> _logger;
        private readonly ISearchingPhoneDbSet _searchingPhoneDb;
        private readonly ISorting<PhoneNumber> _sorting;
        private readonly IPaginating _paginating;

        public PhoneRepos(BlightDbContext blightDbContext, IMapper mapper, IUserContextService userContextService, ILogger<PhoneRepos> logger,
               ISorting<PhoneNumber> sorting, IPaginating paginating, ISearchingPhoneDbSet searchingPhoneDb) : base(blightDbContext, mapper,paginating)
        {
            _mapper = mapper;
            _userContextService = userContextService;
            _logger = logger;
            _sorting = sorting;
            _paginating = paginating;
            _searchingPhoneDb = searchingPhoneDb;
        }
        private User? findActiveUser => _blightDbContext
                    .Users
                    .Include(n => n.BlockedNumbers)
                    .Include(n => n.Role)
                    .SingleOrDefault(x => x.Id == _userContextService.GetUserId);

        private User? activeUser => findActiveUser is null ?
            throw new ForbiddenException("You have not authority for this action") :
            findActiveUser;

        public async override Task<IPagedResult<IDto>> GetAllPaginated(IPaginationObj paginationPhoneQuery)
        {
            var dbResult =_searchingPhoneDb.SearchPhoneDbWithCriteria(_blightDbContext.PhoneNumbers, paginationPhoneQuery);
            var sortingResult = _sorting.Sort(dbResult,paginationPhoneQuery);
            var paginationResult = _paginating.Paginate(sortingResult, paginationPhoneQuery);

            return paginationResult;
        }
        public async Task<IPagedResult<IDto>> GetUserAllBlockedNumbers(IPaginationObj paginationQuery)
        {
            var blockedNumbers = activeUser.BlockedNumbers;
            var mappedCollection = _mapper.Map<ICollection<PhoneNumberViewModel>>(blockedNumbers);
            var queryable = mappedCollection.AsQueryable();
            
            var paginationResult = _paginating.Paginate(queryable, paginationQuery);

            return paginationResult;
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

            if(activeUser.RoleId >1)
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
            else
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

            if(activeUser.RoleId <3)
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
                    .Where(x=>x.Users.Count >= x.IsBullyTreshold)
                    .Select(p=>new PhoneNumberDto
                    {
                        Prefix = p.Prefix,
                        Number = p.Number,
                    })
                    .ToListAsync();

            return allBlockedNumbers;
        }
    }
}
