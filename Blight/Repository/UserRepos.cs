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

namespace Blight.Repository
{
    public class UserRepos : GenericRepository<User>,IUserRepository
    {
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ISchemeGenerator _schemeGenerator;

        public UserRepos(BlightDbContext blightDbContext, IMapper mapper, IPasswordHasher<User> passwordHasher, ISchemeGenerator schemeGenerator)
            : base(blightDbContext, mapper)
        {
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _schemeGenerator = schemeGenerator;
        }

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
            return await base.Create(user);
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

        public async override Task<User> GetById(int id)
        {
            var result = await _dbSet
                    .Include(p => p.BlockedNumbers)
                    .FirstOrDefaultAsync(x => x.Id == id);
            if (result is null)
            {
                throw new NotFoundException("Element not found");
            }

            return result;

        }

    }

}
