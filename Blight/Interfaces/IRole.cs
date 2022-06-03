using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Entieties;

namespace Blight.Interfaces
{
    public interface IRole
    { }
    public interface IAdmin:IRole
    { }
    public interface IUser:IRole
    { }
}
