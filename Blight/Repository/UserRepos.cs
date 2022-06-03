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
using Microsoft.Extensions.Logging;
using Blight.Interfaces.MethodsProvider;


namespace Blight.Repository
{
    public class UserRepos : GenericRepository<User>,IUserRepository
    {
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ISchemeGenerator _schemeGenerator;
        private readonly IUserContextService _userContextService;
        private readonly IAdminPasswordService _adminPasswordService;
        private readonly ILogger<UserRepos> _logger;
        private readonly ISearchingUserDbSet _searchingUserDbSet;
        private readonly ISorting<User> _sorting;
        private readonly IPaginating _paginating;


        public UserRepos(BlightDbContext blightDbContext, IMapper mapper, IPasswordHasher<User> passwordHasher, ISchemeGenerator schemeGenerator,
            IUserContextService userContextService, IAdminPasswordService adminPasswordService, ILogger<UserRepos> logger,
            ISearchingUserDbSet searchingUserDbSet, ISorting<User> sorting, IPaginating paginating)
            : base(blightDbContext, mapper)
        {
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _schemeGenerator = schemeGenerator;
            _userContextService = userContextService;
            _adminPasswordService = adminPasswordService;
            _logger = logger;
            _searchingUserDbSet = searchingUserDbSet;
            _sorting = sorting;
            _paginating = paginating;
        }
        private User? findActiveUser => _blightDbContext
                    .Users
                    .Include(n => n.BlockedNumbers)
                    .Include(n => n.Role)
                    .SingleOrDefault(x => x.Id == _userContextService.GetUserId);

        private User? activeUser => findActiveUser is null ?
            throw new ForbiddenException("Action forbidden") :
            findActiveUser;

        public override async Task<User> Create(IDto dto)
        {
            var user = _mapper.Map<User>(dto);
            string? password = user.Password;

            if(password.Equals(_adminPasswordService.ReadPasswordFromFile()))
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

            if(user.RoleId ==2)
            {
                _logger.LogInformation($"{user.Email} has been registered as admin");
                _adminPasswordService.GenerateAndSavePasswordToDirectoryFromAppSettings();
            }

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

            if(existingUser.RoleId != 3)
            {
                _logger.LogWarning($"{existingUser.Email} logged in as {existingUser.Role}");
            }
            return token;
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
            if(activeUser.RoleId ==3)
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

            return user;
        }
        public async Task<IDto>GetById<T>(int id,T returnedType) where T:IDto
        {
            var iDto = await GetById(id);

            var mappedDto = _mapper.Map<T>(iDto);

            return mappedDto;
        }

        public async override Task Delete(int id)
        {
            if (activeUser.RoleId != 1)
            {
                if (activeUser.Id != id)
                {
                    throw new ForbiddenException("Action forbidden");
                }
            }

            var entity = await FindElement(x => x.Id == id);

            var result = _dbSet.Remove(entity);

            if (result.State != EntityState.Deleted)
            {
                throw new DataBaseException("Something went wrong");
            }
            await _blightDbContext.SaveChangesAsync();
        }

        public async override Task<IPagedResult<IDto>> GetAllPaginated(IPaginationObj paginationQuery)
        {

            var searchingResult =_searchingUserDbSet.SearchUserDbWithCriteria(_dbSet, paginationQuery);
            var sortingResult = _sorting.Sort(searchingResult, paginationQuery);
            var paginationResult = _paginating.Paginate(sortingResult, paginationQuery);

            return paginationResult;

        }

        public async Task<string> BanUser_Change(int id)
        {
            var user =await FindElement(x=>x.Id == id);
            if (user is null)
            {
                throw new NotFoundException("User not found");
            }
            if(user.RoleId == 1)
            {
                throw new ForbiddenException("Action forbidden");
            }

            var userStatus = user.Banned;

            var newUserStatus = !userStatus;
            user.Banned = newUserStatus;

            _blightDbContext.Entry(user)
                            .CurrentValues
                            .SetValues(user);

            await _blightDbContext.SaveChangesAsync();    
            string result = $"User {user.ToString()} " + String.Format("{0}",newUserStatus?"banned":"unbanned");

            _logger.LogInformation($"User {user.ToString()} " + String.Format("{0}", newUserStatus ? "banned" : "unbanned" +
                $"by {activeUser.Email}"));

            return result;
        }
    }

}
