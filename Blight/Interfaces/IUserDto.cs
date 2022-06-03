using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Interfaces
{
    public interface IUserDto:IDto
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        string Nationality { get; set; }
        int RoleId { get; set; }
    }
}
