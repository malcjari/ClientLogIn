using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientLogIn.Models
{
    public class User: IdentityUser<int>
    {
        public string Name { get; set; }
        public string StreetNo { get; set; }
        public string City { get; set; }
        public int ZipCode { get; set; }
    }
}
