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
        public WorkShift workshift { get; set; }

        public List<WorkShift> WorkShiftList { get; set; }

        public DateDataModel dayData { get; set; }

        public User user { get; set; }


        public ViewModel()
        {
            this.workshift = new WorkShift();
            this.WorkShiftList = new List<WorkShift>();
            this.dayData = new DateDataModel();
            this.user = new User();
        }
    }
}
