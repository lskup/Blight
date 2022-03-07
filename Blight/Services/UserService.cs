using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Entieties;
using Blight.Interfaces;
using Blight.Models;
using Blight.Infrastructure;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Blight.Services
{
    public class UserService : IUserService
    {
        private readonly BlightDbContext _blightDbContext;
        private readonly IMapper _mapper;

        public UserService(BlightDbContext blightDbContext, IMapper mapper)
        {
            _blightDbContext = blightDbContext;
            _mapper = mapper;
        }

        public async Task<bool> Delete(int id)
        {
            var user = await _blightDbContext.Users
                .SingleOrDefaultAsync(x => x.Id == id);
                
            if (user is null)
            {
                return false;
            }

            var result = _blightDbContext.Users
                .Remove(user);

            await _blightDbContext.SaveChangesAsync();


            if (result.State == EntityState.Deleted)
                return true;
            else
                return false;

        }

        public async Task<UserViewModelDto> Get(int id)
        {
            var user = await _blightDbContext.Users
                .Include(q => q.BlockedPhoneNumbers)
                .SingleOrDefaultAsync(x => x.Id == id);

            var userViewModel = _mapper.Map<UserViewModelDto>(user);

            if(userViewModel is null)
            {
                return null;
            }

            return userViewModel;

        }

        public async Task<ICollection<User>> GetAll()
        {
            var users = await _blightDbContext.Users
                .ToListAsync();

            return users;
        }

        public async Task<User> Post(UserDto dto)
        {
            var newUser = _mapper.Map<User>(dto);
            var result = _blightDbContext.Users
                .Add(newUser);

            await _blightDbContext.SaveChangesAsync();

            return result.Entity;

        }

        public async Task<bool> Put(int id, UserDto dto)
        {
            var updatedUser = _mapper.Map<User>(dto);

            var locatedUser = await _blightDbContext.Users
                .FirstOrDefaultAsync(x => x.Id == id);

            if(locatedUser is null)
            {
                return false;
            }

            updatedUser.Id = id;

            _blightDbContext.Entry(locatedUser).CurrentValues
                .SetValues(updatedUser);

            await _blightDbContext.SaveChangesAsync();

            return true;

        }
    }
}
