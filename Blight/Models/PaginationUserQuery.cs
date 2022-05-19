using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Enums;

namespace Blight.Models
{
    public class PaginationUserQuery:PaginationQuery
    {
        public bool onlyBannedUsers { get; set; }
        public SortDirection? sortDirection { get; set; } = null;
        public SortUserBy sortUserBy { get; set; }

    }
}
