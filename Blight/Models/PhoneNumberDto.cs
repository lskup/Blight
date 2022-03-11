using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blight.Models
{
    public class PhoneNumberDto
    {
        [Required]
        [RegularExpression("[0-9]{2}",ErrorMessage ="Prefix must consist two digits")]
        public string Prefix { get; set; }
        [Required]
        [RegularExpression("[0-9]{9}", ErrorMessage = "Phone number must consist 9 digits")]
        public string Number { get; set; }

    }
}
