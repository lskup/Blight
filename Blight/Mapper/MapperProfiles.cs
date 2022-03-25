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
            CreateMap<RegisterUserDto, User>()
                .ForMember(c => c.BlockedNumbers, x => x.MapFrom(s => new List<PhoneNumber>()))
                .ForMember(d => d.Email, x => x.MapFrom(j => j.Email.ToLower()))
                .ForMember(e => e.HashedPassword, x => x.MapFrom(w => w.Password));

            CreateMap<User, UserViewModelDto>();
            
            CreateMap<UpdateUserDto, User>();

            CreateMap<PhoneNumber, PhoneNumberDto>();

            CreateMap<PhoneNumberDto, PhoneNumber>();



        }



    }
}
