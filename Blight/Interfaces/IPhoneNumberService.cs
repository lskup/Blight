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
        Task<IEnumerable<PhoneNumber>> GetAll(bool onlyBullyNumbers = false);
        Task<PhoneNumber> Get(int id);
        Task<PhoneNumber> Post(PhoneNumberDto dto);
        Task Delete(int id);

    }
}
