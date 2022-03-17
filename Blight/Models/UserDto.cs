using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Interfaces;

namespace Blight.Models
{
    public class UserDto:IDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [MinLength(7,ErrorMessage ="Password require minimum 7 marks")]
        public string Password { get; set; }
        [Required]
        [Compare("Password",ErrorMessage ="Password confirmation doesn't match password")]
        public string PasswordConfirmation { get; set; }
        [DataType(DataType.Date,ErrorMessage ="Enter Date as yyyy-MM-dd")]
        public DateTime DateOfBirth { get; set; }
        public string Nationality { get; set; }
    }
}
