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
    public class UserRepos2 : GenericRepository2<User>
    {
        private readonly IMapper _mapper;

        public UserRepos2(BlightDbContext blightDbContext, IMapper mapper) : base(blightDbContext,mapper)
        {
            _mapper = mapper;
        }

        public override async Task<User> Update(int id,IDto dto)
        {
            var user = _mapper.Map<User>(dto);

            var existingUser = await FindElement
                (e => e.Id == id);

            if (existingUser == null)
            {
                throw new NotFoundException("User not Found");
            }

            //existingUser.FirstName = user.FirstName;
            //existingUser.LastName = user.LastName;
            //existingUser.DateOfBirth = user.DateOfBirth;
            //existingUser.Nationality = user.Nationality;

            var dtoProperties = user.GetType()
                .GetProperties();

            var existingUserProperties = existingUser.GetType()
                .GetProperties();

            for (int i = 0; i < existingUserProperties.Length; i++)
            {
                if (dtoProperties[i].GetValue(user) != null)
                {
                    existingUserProperties.SetValue(dtoProperties[i].GetValue(user), i);
                }
            }


            _blightDbContext.Entry(existingUser)
                .CurrentValues
                .SetValues(existingUser);

            await _blightDbContext.SaveChangesAsync();

            return existingUser;
        }
        //////////////////////////////////////////////////////////////////////////






    }



}
