using Blight.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Models
{
    public class LoginUserDto:IDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
