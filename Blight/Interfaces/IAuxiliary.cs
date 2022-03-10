using Blight.Entieties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Interfaces
{
    public interface IAuxiliary<T>
    {
        Task<bool> UpdateIfExist(T genericObject);
        Task TryAddToDb(T genericObject);
        Task<T> FindById(int id);

    }
}
