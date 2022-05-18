using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Models
{
    public class PaginationUserQuery:PaginationQuery
    {
        public bool onlyBannedUsers { get; set; }
    }
}
