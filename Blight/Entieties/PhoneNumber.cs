using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Interfaces;

namespace Blight.Entieties
{
    public class PhoneNumber:IDto,IEntity
    {
        public int Id { get; set; }
        public string Prefix { get; set; }
        public string Number { get; set; }
        public int IsBullyTreshold { get; set; } = 20;
        public virtual ICollection<User> Users { get; set; }

    }

}
