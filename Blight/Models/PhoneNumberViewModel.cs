using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Interfaces;

namespace Blight.Models
{
    public class PhoneNumberViewModel:IPhoneNumberDto
    {
        public string Prefix { get; set; }
        public string Number { get; set; }
        public int Notified { get; set; }

    }
}
