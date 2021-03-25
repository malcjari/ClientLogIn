using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ClientLogIn.Models
{
    public class User: IdentityUser<int>
    {
        [DisplayName("Namn")]
        public string Name { get; set; }
        [DisplayName("Gata")]
        public string StreetNo { get; set; }
        [DisplayName("Stad")]
        public string City { get; set; }
        [DisplayName("Postnummer")]
        public int ZipCode { get; set; }
    }
}
