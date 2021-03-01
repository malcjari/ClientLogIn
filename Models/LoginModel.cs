using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ClientLogIn.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Please Enter username")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Please Enter password")]
        [Display(Name ="Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
