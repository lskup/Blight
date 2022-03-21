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

            CreateMap<UserDto, User>()
                .ForMember(c => c.BlockedNumbers, x => x.MapFrom(s => new List<PhoneNumber>()));

            CreateMap<User, UserViewModelDto>();
            
            CreateMap<UserUpdateDto, User>();

            CreateMap<PhoneNumber, PhoneNumberDto>();

            CreateMap<PhoneNumberDto, PhoneNumber>();



        }



    }
}
