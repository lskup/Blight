using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Interfaces
{
    public interface IPhoneNumberDto:IDto
    {
        public string Prefix { get; set; }
        public string Number { get; set; }
        public int Notified { get; set; }
    }
}
