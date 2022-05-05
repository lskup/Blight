using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Models
{
    public class PaginationPhoneQuery:PaginationQuery
    {
        public bool onlyBlockedNumbers { get; set; } = false;
    }
}
