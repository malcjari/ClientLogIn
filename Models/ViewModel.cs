using ClientLogIn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientLogIn.Models;

namespace ClientLogIn.Models
{
    public class ViewModel
    {
        public WorkShift WorkShift { get; set; }

        public List<WorkShift> WorkShiftList { get; set; }

        public DateDataModel dayData { get; set; }

        public User user { get; set; }


        public ViewModel()
        {
            this.WorkShift = new WorkShift();
            this.WorkShiftList = new List<WorkShift>();
            this.dayData = new DateDataModel();
            this.user = new User();
        }
    }
}
