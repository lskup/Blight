using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Entieties
{
    public class PhoneNumber
    {
        public int Id { get; set; }
        public string Prefix { get; set; }
        public string Number { get; set; }
        public int Notified { get; set; } = 1;
        public bool IsBully { get; set; } = false;

    }
}
