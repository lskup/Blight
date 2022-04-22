using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Entieties;

namespace Blight.Interfaces
{
    public interface IPhoneRepository:IGenericRepository<PhoneNumber>
    {
        Task<IEnumerable<IDto>> GetUserBlockedNumbers();
    }
}
