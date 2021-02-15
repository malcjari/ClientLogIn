using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientLogIn.Models
{
    public class LoginResultModel
    {
        public bool Status { get; set; }
        public List<string> Role { get; set; }

        public LoginResultModel()
        {
            this.Role = new List<string>();
        }
    }
}
