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
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        public string PasswordConfirmation { get; set; }
        [DataType(DataType.Date),DisplayFormat(DataFormatString ="yyyy-MM-dd")]
        public DateTime DateOfBirth { get; set; }
        public string Nationality { get; set; }
    }
}
