using Blight.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Interfaces
{
    public interface IUserPagination
    {
         bool onlyBannedUsers { get; set; }
         SortDirection? sortDirection { get; set; }
         SortUserBy sortUserBy { get; set; }
    }
}
