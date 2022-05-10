using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blight.Interfaces;

namespace Blight.Models
{
    public class PhoneNumberDto:IDto
    {
        [Required]
        [RegularExpression("[0-9]{2}",ErrorMessage ="Prefix must consist two digits")]
        public string? Prefix { get; set; }
        [Required]
        [RegularExpression("[0-9]{7,10}", ErrorMessage = "Phone number must consist from 7 to 10 digits")]
        public string? Number { get; set; }

    }
}
