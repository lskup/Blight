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
            //CreateMap<RegisterUserDto, User>()
            //    .ForMember(c => c.BlockedNumbers, x => x.MapFrom(s => new List<PhoneNumber>()))
            //    .ForMember(d => d.Email, x => x.MapFrom(j => j.Email.ToLower()));

            CreateMap<RegisterUserDto, User>()
                .ForMember(d => d.Email, x => x.MapFrom(j => j.Email.ToLower()));


            CreateMap<User, GetAllUserViewModel>();
            CreateMap<User, GetByIdUserViewModel>();


            CreateMap<UpdateUserDto, User>();

            CreateMap<PhoneNumber, PhoneNumberDto>();

            CreateMap<PhoneNumberDto, PhoneNumber>()
                .ForMember(x => x.Users, m => m.MapFrom(x => new List<User>()));
               

            CreateMap<PhoneNumber, PhoneNumberViewModel>();
            CreateMap<PhoneNumber, AdminPhoneNumberViewModel>();

            CreateMap<Role, RoleViewModel>();


            CreateMap<LoginUserDto,User>()
                .ForMember(d => d.Email, x => x.MapFrom(j => j.Email.ToLower()));



        }



    }
}
