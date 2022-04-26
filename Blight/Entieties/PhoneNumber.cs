﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Interfaces;

namespace Blight.Entieties
{
    public class PhoneNumber:IDto
    {
        public int Id { get; set; }
        public string Prefix { get; set; }
        public string Number { get; set; }
        public int? Notified { get { return Users.Count; } }
        public bool? IsBully
        {
            get
            {
                if(Notified >20)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public ICollection<User> Users { get; set; }
    }
}
