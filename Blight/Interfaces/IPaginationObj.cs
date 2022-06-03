using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Interfaces
{
    public interface IPaginationObj
    {
         string? SearchPhrase { get; set; }
         int PageNumber { get; set; }
         int PageSize { get; set; }

    }
}
