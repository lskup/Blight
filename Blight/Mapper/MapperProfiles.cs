using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Entieties;
using Blight.Models;
using Blight.Interfaces;

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
                .ForMember(d => d.Email,
                x => x.MapFrom(j => j.Email.ToLower()));
            CreateMap<GetByIdUserViewModel, User>();

            CreateMap<User, GetAllUserViewModel>();
            CreateMap<User, GetByIdUserViewModel>();


            CreateMap<UpdateUserDto, User>();

            CreateMap<PhoneNumber, PhoneNumberDto>();

            CreateMap<PhoneNumberDto, PhoneNumber>()
                .ForMember(x => x.Users,
                m => m.MapFrom(x => new List<User>()));
               
            CreateMap<PhoneNumber, PhoneNumberViewModel>()
                .ForMember(x=>x.Notified,
                p=>p.MapFrom(x=>x.Users.Count()));

            CreateMap<PhoneNumber, AdminPhoneNumberViewModel>()
                .ForMember(x => x.Notified,
                p => p.MapFrom(x => x.Users.Count()));

            CreateMap<AdminPhoneNumberViewModel, PhoneNumberViewModel>();

            CreateMap<Role, RoleViewModel>();


            CreateMap<LoginUserDto,User>()
                .ForMember(d => d.Email, x => x.MapFrom(j => j.Email.ToLower()));

            CreateMap<IPagedResult<AdminPhoneNumberViewModel>, IPagedResult<PhoneNumberViewModel>>()
                .ForMember(x => x.Items,
                p => p.MapFrom
                (x => x.Items.Select(c => new PhoneNumberViewModel
                {
                    Prefix = c.Prefix,
                    Number = c.Number,
                    Notified = c.Notified
                })));
               


        }



    }
}
