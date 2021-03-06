using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Enums;
using Blight.Interfaces;

namespace Blight.Models
{
    public class PaginationPhoneQuery:PaginationQuery,IPhonePagination
    {
        public bool onlyBullyNumbers { get; set; } = false;

        public SortDirection? sortDirection { get; set; } = null;

    }
}
