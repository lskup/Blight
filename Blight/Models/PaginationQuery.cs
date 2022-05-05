using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Interfaces;

namespace Blight.Models
{
    public class PaginationQuery:IPagination
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
