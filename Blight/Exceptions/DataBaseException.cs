using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Exceptions
{
    public class DataBaseException:Exception
    {
        public DataBaseException(string message):base(message)
        {
        }
    }
}
