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

namespace Blight.Repository
{
    public class UserRepos2 : GenericRepository2<User>,IUserRepository
    {
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<RegisterUserDto> _passwordHasher;

        public UserRepos2(BlightDbContext blightDbContext, IMapper mapper, IPasswordHasher<RegisterUserDto> passwordHasher) : base(blightDbContext, mapper)
        {
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public override async Task<User> Create(IDto dto)
        {
            var user = _mapper.Map<RegisterUserDto>(dto);
            var password = user.Password;

            var hashedPassword = _passwordHasher.HashPassword(user, password);
            user.Password = hashedPassword;
            return await base.Create(user);
        }

        public async Task Login(IDto dto)
        {
            throw new NotImplementedException();
        }

        public async Task<User> Register(IDto dto)
        {
            return await Create(dto);

        }

        public override async Task<User> Update(int id,IDto dto)
        {
            //var user = _mapper.Map<User>(dto);

            var user = dto;

            var existingUser = await FindElement
                (e => e.Id == id);

            if (existingUser == null)
            {
                throw new NotFoundException("User not Found");
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
                        var propertyValue = dtoProperties[j].GetValue(user);

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

    }

}
