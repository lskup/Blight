﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Entieties
{
    public class PhoneNumber
    {
        public int Id { get; set; }
        public int Prefix { get; set; }
        public int Number { get; set; }
        public int Notified { get; set; }
        public bool IsBully
        {
            get
            {
                if(Notified<=20)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
