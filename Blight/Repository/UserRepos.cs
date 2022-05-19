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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Blight.Authentication;
using Blight.Enums;

namespace Blight.Repository
{
    public class UserRepos : GenericRepository<User>,IUserRepository
    {
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ISchemeGenerator _schemeGenerator;
        private readonly IUserContextService _userContextService;


        public UserRepos(BlightDbContext blightDbContext, IMapper mapper, IPasswordHasher<User> passwordHasher, ISchemeGenerator schemeGenerator, IUserContextService userContextService)
            : base(blightDbContext, mapper)
        {
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _schemeGenerator = schemeGenerator;
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

        public override async Task<User> Create(IDto dto)
        {
            var user = _mapper.Map<User>(dto);
            string? password = user.Password;

            if(password.Equals("Admin123!"))
            {
                user.RoleId = 2;
            }

            string? hashedPassword = _passwordHasher.HashPassword(user, password);
            user.Password = hashedPassword;

            var result = await _dbSet.AddAsync(user);

            if (result.State != EntityState.Added)
            {
                throw new DataBaseException("Something went wrong");
            }

            await _blightDbContext.SaveChangesAsync();

            return user;

        }

        public async Task<string> Login(IDto dto)
        {
            var mappedDto = _mapper.Map<User>(dto);
            var existingUser = await FindElement(x => x.Email == mappedDto.Email);

            if(existingUser is null)
            {
                throw new BadRequestException("email or password invalid");
            }

            string password = mappedDto.Password;

            var hashedPasswordAccordance = _passwordHasher.VerifyHashedPassword(existingUser, existingUser.Password, password);

            if(hashedPasswordAccordance == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("email or password invalid");
            }

            var token = _schemeGenerator.GenerateJWT(existingUser);

            return token;
        }

        public override async Task<User> Update(int id, IDto dto)
        {
            if(id!=activeUser.Id)
            {
                throw new ForbiddenException("You have not authority for this action");
            }

            var user = dto;

            var existingUser = await FindElement(
                e => e.Id == id);

            if (existingUser == null)
            {
                throw new NotFoundException("User not found");
            }

            var dtoProperties = user.GetType()
                .GetProperties();

            var existingUserProperties = existingUser.GetType()
                .GetProperties();

            for (int i = 0; i < existingUserProperties.Length; i++)
            {
                for (int j = 0; j < dtoProperties.Length; j++)
                {
                    if(existingUserProperties[i].Name == dtoProperties[j].Name)
                    {
                        object? propertyValue = dtoProperties[j].GetValue(user);

                        if(propertyValue != null)
                        {
                            existingUserProperties[i].SetValue(existingUser, propertyValue);
                        }
                    }
                }
            }

            _blightDbContext.Entry(existingUser)
                .CurrentValues
                .SetValues(existingUser);

            await _blightDbContext.SaveChangesAsync();

            return existingUser;
        }

        public async override Task<User> FindElement(Expression<Func<User, bool>> predicate)
        {
            var result = await _dbSet
                .Include(p=>p.BlockedNumbers)
                .Include(x => x.Role)
                .SingleOrDefaultAsync(predicate);

            return result;
        }

        public async override Task<IDto> GetById(int id)
        {
            if(activeUser.RoleId ==1)
            {
                if(activeUser.Id != id)
                {
                    throw new ForbiddenException("Action forbidden");
                }
            }

            var user = await _dbSet
                    .Include(p => p.BlockedNumbers)
                    .Include(r=>r.Role)
                    .FirstOrDefaultAsync(x => x.Id == id);

            if (user is null)
            {
                throw new NotFoundException("User not found");
            }

            var result = _mapper.Map<GetByIdUserViewModel>(user);

            return result;

        }

        public async override Task Delete(int id)
        {
            if (id != activeUser.Id)
            {
                throw new ForbiddenException("You have not authority for this action");
            }

            var entity = await FindElement(x => x.Id == id);

            var result = _dbSet.Remove(entity);

            if (result.State != EntityState.Deleted)
            {
                throw new DataBaseException("Something went wrong");
            }
            await _blightDbContext.SaveChangesAsync();
        }

        public async override Task<IPagedResult<IDto>> GetAllPaginated(IPagination paginationQuery)
        {
            var paginationObj = paginationQuery as PaginationUserQuery;

            var entryList = _dbSet
                .AsNoTracking()
                .Where(r => paginationObj.SearchPhrase == null ||
                    (r.FirstName.ToLower().Contains(paginationObj.SearchPhrase.ToLower())) ||
                    (r.LastName.ToLower().Contains(paginationObj.SearchPhrase.ToLower())) ||
                    (r.Nationality.ToLower().Contains(paginationObj.SearchPhrase.ToLower())));
                

            Dictionary<SortUserBy, Expression<Func<User, object>>> userSortOptions_userProperties_Pairs = new Dictionary<SortUserBy, Expression<Func<User, object>>>
            {
                {SortUserBy.FirstName,p=>p.FirstName},
                {SortUserBy.LastName,p=>p.LastName},
                {SortUserBy.Nationality,p=>p.Nationality},
                {SortUserBy.RoleId,p=>p.RoleId},
            };

            var selected = userSortOptions_userProperties_Pairs[paginationObj.sortUserBy];
            
            entryList = paginationObj.sortDirection == SortDirection.Asc
                                            ? entryList.OrderBy(selected)
                                            : entryList.OrderByDescending(selected);

            if (paginationObj.onlyBannedUsers == true)
            {
                entryList = entryList.Where(x => x.Banned == true);
            }


            var paginatedList = entryList
                .Skip(paginationObj.PageSize * (paginationObj.PageNumber - 1))
                .Take(paginationObj.PageSize)
                .ToList();

            var result = _mapper.Map<IEnumerable<GetAllUserViewModel>>(paginatedList);

            var recordsTotal = entryList.Count();

            var pageResult =
                new PagedResult<IDto>(result, recordsTotal, paginationObj.PageSize, paginationObj.PageNumber);

            return pageResult;

        }

        public async Task<string> BanUser_Change(int id)
        {
            var user =await FindElement(x=>x.Id == id);
            if (user is null)
            {
                throw new NotFoundException("User not found");
            }

            var userStatus = user.Banned;

            var newUserStatus = !userStatus;
            user.Banned = newUserStatus;

            _blightDbContext.Entry(user)
                            .CurrentValues
                            .SetValues(user);

            await _blightDbContext.SaveChangesAsync();    
            string result = $"User {user.ToString()} " + String.Format("{0}",newUserStatus?"banned":"unbanned");

            return result;
        }
    }

}
