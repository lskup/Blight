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
    public class UserRepository : GenericRepository<User>
    {
        private readonly IMapper _mapper;

        public UserRepository(BlightDbContext blightDbContext, IMapper mapper) : base(blightDbContext)
        {
            _mapper = mapper;
        }

        public override async Task<User> CreateOrUpdate(int? id,IDto dto)
        {
            var user = _mapper.Map<User>(dto);

            var existingUser = await FindElement
                (e => e.Id == id.Value);

            if (existingUser is null)
            {
                throw new NotFoundException("User not Found");
            }

            user.Id = existingUser.Id;

            _blightDbContext.Entry(existingUser)
                .CurrentValues
                .SetValues(user);

            await _blightDbContext.SaveChangesAsync();

            return user;
        }
    }



}
