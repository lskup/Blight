using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Interfaces;
using Blight.Entieties;
using Blight.Models;

namespace Blight.Models
{
    public class GetByIdUserModel:IDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nationality { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public virtual ICollection<PhoneNumberViewModel> BlockedNumbers { get; set; }
        public RoleViewModel Role { get; set; }


    }
}
