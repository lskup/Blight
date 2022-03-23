using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Interfaces;

namespace Blight.Models
{
    public class RegisterUserDto : IDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
        [DataType(DataType.Date,ErrorMessage ="Enter Date as yyyy-MM-dd"),DisplayFormat(DataFormatString ="yyyy-MM-dd")]
        public DateTime DateOfBirth { get; set; }
        public string Nationality { get; set; }
    }
}
