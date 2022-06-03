using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Interfaces;

namespace Blight.Entieties
{
    public class User:IDto,IEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nationality { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public virtual ICollection<PhoneNumber> BlockedNumbers { get; set; }
        public int RoleId { get; set; } = 3;
        public virtual Role Role { get; set; }
        public bool Banned { get; set; }

        public override string ToString()
        {
            return Email;
        }


    }
}
