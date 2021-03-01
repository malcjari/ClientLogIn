using ClientLogIn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace ClientLogIn.Models
{
    public class ViewModel
    {
        public ArbetsPass arbetspass { get; set; }

        public List<ArbetsPass> arbetspassList { get; set; }

        public DateDataModel dayData { get; set; }

        public User user { get; set; }


        public ViewModel()
        {
            this.arbetspass = new ArbetsPass();
            this.arbetspassList = new List<ArbetsPass>();
            this.dayData = new DateDataModel();
            this.user = new User();
        }
    }
}
