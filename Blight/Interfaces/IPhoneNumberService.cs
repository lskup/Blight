using Blight.Entieties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Models;

namespace Blight.Interfaces
{
    public interface IPhoneNumberService
    {
        Task<ICollection<PhoneNumber>> GetAll(bool onlyBullyNumbers = false);
        Task<PhoneNumberDto> Get(int id);
        Task<bool> Put(int id, PhoneNumberDto dto);
        Task<PhoneNumber> Post(PhoneNumberDto dto);
        Task<bool> Delete(int id);

    }
}
