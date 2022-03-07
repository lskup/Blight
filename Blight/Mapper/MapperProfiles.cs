using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Entieties;
using Blight.Models;


namespace Blight.Mapper
{
    public class MapperProfiles:Profile
    {
        public MapperProfiles()
        {
            CreateMap<User, UserDto>();

            CreateMap<UserDto, User>();

            CreateMap<User, UserViewModelDto>();

            CreateMap<PhoneNumber, PhoneNumberDto>();

            CreateMap<PhoneNumberDto, PhoneNumber>();



        }



    }
}
