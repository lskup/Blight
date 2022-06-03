using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Interfaces
{
    public interface IPagedResult<T>
    {
         IEnumerable<T> Items { get; set; }
         int TotalPages { get; set; }
         int ItemsFrom { get; set; }
         int ItemsTo { get; set; }
         int TotalItemsCount { get; set; }
    }
}
