using Blight.Entieties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Models
{
    public class UserViewModelDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<PhoneNumber> BlockedPhoneNumbers { get; set; }

    }
}
