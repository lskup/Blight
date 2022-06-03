using Blight.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Interfaces
{
    public interface IPhonePagination:IPaginationObj
    {
         bool onlyBullyNumbers { get; set; }

         SortDirection? sortDirection { get; set; }
    }
}
