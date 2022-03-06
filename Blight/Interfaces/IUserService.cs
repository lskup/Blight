using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Entieties;
using Blight.Models;

namespace Blight.Interfaces
{
    public interface IUserService
    {
        Task<ICollection<User>> GetAll();
        Task<User> Get(int id);
        Task<bool> Put(int id,UserDto dto);
        Task<User> Post(UserDto dto);
        Task<bool> Delete(int id);


    }
}
