using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Entieties;
using Blight.Models;
using Blight.Interfaces;

namespace Blight.Interfaces
{
    public interface IUserRepository:IGenericRepository2<User>
    {
        Task Login(IDto dto);
        Task Register(IDto dto);

    }
}
