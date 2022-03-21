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

            user.Id = existingUser.Id;

            var mappedObjProperties = user.GetType()
                .GetProperties();

            var existingObjProperties = existingUser.GetType()
                .GetProperties();


            for (int j = 0; j < mappedObjProperties.Length; j++)
            {
                for (int i = 0; i < existingObjProperties.Length; i++)
                {
                    if(mappedObjProperties[j].Name == existingObjProperties[i].Name)
                    {
                        existingObjProperties.SetValue(mappedObjProperties[j], i);
                    }
                }
            }

            _blightDbContext.Entry(existingUser)
                .CurrentValues
                .SetValues(user);

            await _blightDbContext.SaveChangesAsync();

            return existingUser;
        }
        //////////////////////////////////////////////////////////////////////////






    }



}
